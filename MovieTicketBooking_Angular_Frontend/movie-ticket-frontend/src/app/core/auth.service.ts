import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthUser } from './models';

interface ApiEnvelope<T> { success: boolean; data: T; errors?: string[]; }
interface RegisterPayload { fullName: string; email: string; phoneNumber: string; userName: string; password: string; }
interface LoginPayload { userNameOrEmail: string; password: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly storageKey = 'reeltime.auth';
  private readonly current = signal<AuthUser | null>(this.readStoredUser());
  readonly user = this.current.asReadonly();
  readonly isLoggedIn = computed(() => !!this.current()?.accessToken);
  readonly isAdmin = computed(() => this.current()?.roles?.some(r => r.toLowerCase() === 'admin') ?? false);

  constructor(private http: HttpClient) {}

  register(payload: RegisterPayload): Observable<ApiEnvelope<unknown>> {
    return this.http.post<ApiEnvelope<unknown>>(`${environment.apiUrl}/auth/register`, payload);
  }

  login(payload: LoginPayload): Observable<ApiEnvelope<AuthUser>> {
    return this.http.post<ApiEnvelope<AuthUser>>(`${environment.apiUrl}/auth/login`, payload).pipe(
      tap(response => {
        if (response?.data?.accessToken) {
          this.current.set(response.data);
          localStorage.setItem(this.storageKey, JSON.stringify(response.data));
        }
      })
    );
  }

  logout(): void { this.current.set(null); localStorage.removeItem(this.storageKey); }
  token(): string | null { return this.current()?.accessToken ?? null; }

  private readStoredUser(): AuthUser | null {
    try {
      const raw = localStorage.getItem(this.storageKey);
      if (!raw) return null;
      const user = JSON.parse(raw) as AuthUser;
      if (!user.accessToken || (user.expiresAt && new Date(user.expiresAt).getTime() <= Date.now())) {
        localStorage.removeItem(this.storageKey); return null;
      }
      return user;
    } catch { return null; }
  }
}
