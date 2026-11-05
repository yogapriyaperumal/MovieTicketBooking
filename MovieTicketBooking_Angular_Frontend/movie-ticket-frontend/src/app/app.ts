import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth.service';

@Component({ selector: 'app-root', standalone: true, imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive], templateUrl: './app.html' })
export class App {
  constructor(public auth: AuthService) {}
  logout(): void { this.auth.logout(); window.location.href = '/'; }
}
