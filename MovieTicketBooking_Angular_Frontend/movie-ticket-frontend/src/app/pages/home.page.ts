import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../core/api.service';
import { AuthService } from '../core/auth.service';
import { Movie } from '../core/models';

@Component({ selector: 'app-home-page', standalone: true, imports: [CommonModule, FormsModule, RouterLink], template: `
<section class="hero">
  <div class="hero-copy"><span class="eyebrow"><i></i> YOUR NEXT MOVIE NIGHT STARTS HERE</span>
    <h1>Stories worth<br><em>showing up for.</em></h1>
    <p>Find your film, pick your seats, and make a night of it.</p>
    <a class="btn btn-primary" href="#movies">Explore movies <span>↗</span></a>
  </div>
  <div class="hero-art"><div class="ticket ticket-back"></div><div class="ticket ticket-front"><span>REELTIME</span><b>ADMIT<br>ONE</b><small>GOOD TIMES AHEAD</small></div><div class="orb"></div></div>
  <div class="hero-bottom"><span>THE BIG SCREEN FEELS BETTER</span><span>01 / DISCOVER · 02 / SELECT · 03 / ENJOY</span></div>
</section>
<section id="movies" class="section">
  <div class="section-heading"><div><span class="eyebrow">NOW SHOWING</span><h2>Find your <em>next favourite.</em></h2></div>
    <div class="search-box"><span>⌕</span><input [(ngModel)]="query" (keyup.enter)="search()" placeholder="Search by movie title"><button (click)="search()">Search</button></div>
  </div>
  <div *ngIf="!auth.isLoggedIn()" class="notice"><span>✦</span><div><b>Sign in to explore the full movie catalogue</b><p>Movie listing is protected by your backend's AdminOrCustomer policy. You can still search by title below.</p></div><a routerLink="/login">Sign in →</a></div>
  <div *ngIf="loading" class="loading">Loading movies…</div>
  <div *ngIf="error" class="error-banner">{{error}}</div>
  <div class="movie-grid" *ngIf="movies.length">
    <a class="movie-card" *ngFor="let movie of movies" [routerLink]="['/movies', movie.movieId]">
      <div class="poster" [attr.data-tone]="movie.movieId % 6"><span class="poster-mark">RT</span><span class="poster-title">{{movie.title}}</span><span class="poster-genre">{{movie.genre}}</span></div>
      <div class="movie-meta"><div><h3>{{movie.title}}</h3><p>{{movie.language}} · {{movie.durationMinutes}} min</p></div><span class="round-arrow">↗</span></div>
    </a>
  </div>
  <div *ngIf="!loading && !movies.length && !error" class="empty-state"><div>▣</div><h3>No movies to show yet</h3><p>Sign in or search for a movie title.</p></div>
</section>
<section class="how-section"><span class="eyebrow">THE REELTIME WAY</span><h2>Three steps to <em>great plans.</em></h2><div class="steps"><article><b>01</b><h3>Pick a movie</h3><p>Browse what's showing and find your kind of story.</p></article><article><b>02</b><h3>Choose your seats</h3><p>See the live seat layout and select your favourites.</p></article><article><b>03</b><h3>Enjoy the show</h3><p>Confirm your booking and get ready for the big screen.</p></article></div></section>
` })
export class HomePage implements OnInit {
  movies: Movie[] = []; query = ''; loading = false; error = '';
  constructor(private api: ApiService, public auth: AuthService) {}
  ngOnInit(): void { if (this.auth.isLoggedIn()) this.load(); }
  load(): void { this.loading = true; this.error = ''; this.api.movies().subscribe({ next: r => { this.movies = r.items ?? []; this.loading = false; }, error: e => { this.error = e.status === 401 ? 'Please sign in to browse movies.' : this.message(e); this.loading = false; } }); }
  search(): void { if (!this.query.trim()) { if (this.auth.isLoggedIn()) this.load(); return; } this.loading = true; this.error = ''; this.api.searchMovies(this.query).subscribe({ next: r => { this.movies = r.items ?? []; this.loading = false; }, error: e => { this.error = this.message(e); this.loading = false; } }); }
  private message(e: any): string { return e.error?.message || e.error?.errors?.join(', ') || 'Could not load movies. Check that the backend is running.'; }
}
