import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../core/auth.service';

@Component({ selector: 'app-login-page', standalone: true, imports: [CommonModule, FormsModule, RouterLink], template: `
<div class="auth-layout"><div class="auth-aside"><span class="eyebrow">WELCOME BACK</span><h1>Your seat<br>is <em>waiting.</em></h1><p>Sign in and get back to the stories you love.</p><div class="auth-stamp">RT <small>REELTIME CINEMA CLUB</small></div></div>
<form class="auth-card" (ngSubmit)="submit()"><span class="eyebrow">MEMBER ACCESS</span><h2>Sign in</h2><p class="muted">Enter your account details to continue.</p>
<label>Email or username<input name="userNameOrEmail" [(ngModel)]="userNameOrEmail" required autocomplete="username" placeholder="you@example.com"></label>
<label>Password<input name="password" [(ngModel)]="password" required type="password" autocomplete="current-password" placeholder="Your password"></label>
<div class="error-banner" *ngIf="error">{{error}}</div><button class="btn btn-primary full" [disabled]="busy">{{busy ? 'Signing in…' : 'Sign in'}} <span>→</span></button>
<p class="auth-switch">New to ReelTime? <a routerLink="/register">Create an account</a></p></form></div>` })
export class LoginPage {
  userNameOrEmail = ''; password = ''; busy = false; error = '';
  constructor(private auth: AuthService, private router: Router) {}
  submit(): void { this.busy = true; this.error = ''; this.auth.login({ userNameOrEmail: this.userNameOrEmail, password: this.password }).subscribe({ next: () => this.router.navigateByUrl('/'), error: e => { this.error = e.error?.errors?.join(', ') || e.error?.message || 'Login failed. Check your credentials and API connection.'; this.busy = false; } }); }
}
