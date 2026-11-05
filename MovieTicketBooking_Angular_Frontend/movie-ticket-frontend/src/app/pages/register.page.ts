import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../core/auth.service';

@Component({ selector: 'app-register-page', standalone: true, imports: [CommonModule, FormsModule, RouterLink], template: `
<div class="auth-layout"><div class="auth-aside"><span class="eyebrow">JOIN THE CLUB</span><h1>Good films.<br><em>Great memories.</em></h1><p>Create your account and make every movie night count.</p><div class="auth-stamp">RT <small>REELTIME CINEMA CLUB</small></div></div>
<form class="auth-card" (ngSubmit)="submit()"><span class="eyebrow">CREATE ACCOUNT</span><h2>Join ReelTime</h2><p class="muted">A few details and you're in.</p>
<label>Full name<input name="fullName" [(ngModel)]="form.fullName" required minlength="3" maxlength="100" placeholder="Your full name"></label>
<label>Email address<input name="email" [(ngModel)]="form.email" required type="email" placeholder="you@example.com"></label>
<label>Phone number<input name="phoneNumber" [(ngModel)]="form.phoneNumber" required type="tel" placeholder="Your phone number"></label>
<label>Username<input name="userName" [(ngModel)]="form.userName" required minlength="3" placeholder="Choose a username"></label>
<label>Password<input name="password" [(ngModel)]="form.password" required type="password" minlength="8" placeholder="8+ characters, upper/lowercase, number & symbol"></label>
<div class="error-banner" *ngIf="error">{{error}}</div><div class="success-banner" *ngIf="success">{{success}}</div>
<button class="btn btn-primary full" [disabled]="busy">{{busy ? 'Creating account…' : 'Create account'}} <span>→</span></button><p class="auth-switch">Already a member? <a routerLink="/login">Sign in</a></p></form></div>` })
export class RegisterPage {
  form = { fullName: '', email: '', phoneNumber: '', userName: '', password: '' }; busy = false; error = ''; success = '';
  constructor(private auth: AuthService, private router: Router) {}
  submit(): void { this.busy = true; this.error = ''; this.success = ''; this.auth.register(this.form).subscribe({ next: r => { this.success = 'Account created. Please sign in.'; this.busy = false; setTimeout(() => this.router.navigateByUrl('/login'), 900); }, error: e => { this.error = e.error?.errors?.join(', ') || e.error?.message || 'Registration failed. Please check your details.'; this.busy = false; } }); }
}
