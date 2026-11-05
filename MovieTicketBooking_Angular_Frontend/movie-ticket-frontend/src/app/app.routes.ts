import { Routes } from '@angular/router';
import { HomePage } from './pages/home.page';
import { LoginPage } from './pages/login.page';
import { RegisterPage } from './pages/register.page';
import { MovieDetailPage } from './pages/movie-detail.page';
import { BookingPage } from './pages/booking.page';
import { AdminPage } from './pages/admin.page';
import { adminGuard, authGuard } from './core/guards';

export const routes: Routes = [
  { path: '', component: HomePage, title: 'ReelTime — Movie Tickets' },
  { path: 'login', component: LoginPage, title: 'Sign in — ReelTime' },
  { path: 'register', component: RegisterPage, title: 'Join ReelTime' },
  { path: 'movies/:id', component: MovieDetailPage, title: 'Movie details — ReelTime' },
  { path: 'book/:showId', component: BookingPage, canActivate: [authGuard], title: 'Choose seats — ReelTime' },
  { path: 'admin', component: AdminPage, canActivate: [authGuard, adminGuard], title: 'Admin — ReelTime' },
  { path: '**', redirectTo: '' }
];
