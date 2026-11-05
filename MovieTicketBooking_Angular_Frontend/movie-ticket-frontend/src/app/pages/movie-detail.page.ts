import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService } from '../core/api.service';
import { Movie, Show } from '../core/models';

@Component({ selector: 'app-movie-detail-page', standalone: true, imports: [CommonModule, RouterLink], template: `
<div class="page-wrap" *ngIf="movie as m"><a class="back-link" routerLink="/">← Back to movies</a><section class="detail-hero"><div class="detail-poster" [attr.data-tone]="m.movieId % 6"><span>REELTIME ORIGINAL FEEL</span><b>{{m.title}}</b><small>{{m.genre}}</small></div><div class="detail-copy"><span class="eyebrow">NOW SHOWING</span><h1>{{m.title}}</h1><p class="detail-desc">{{m.description || 'Get ready for a cinematic experience. Choose a showtime below and book your seats.'}}</p><div class="detail-facts"><span>{{m.language}}</span><span>{{m.durationMinutes}} min</span><span>{{m.genre}}</span><span>{{m.releaseDate | date:'yyyy'}}</span></div><h3>Choose a showtime</h3><div *ngIf="loading" class="loading">Finding showtimes…</div><div *ngIf="error" class="error-banner">{{error}}</div><div class="show-list" *ngIf="shows.length"><article class="show-option" *ngFor="let show of shows"><div><b>{{show.theatreName}}</b><p>{{show.screenName}} · {{show.startTime | date:'EEE, d MMM · h:mm a'}}</p></div><div class="show-price"><b>{{show.ticketPrice | currency:'INR':'symbol':'1.0-0'}}</b><a class="btn btn-small btn-primary" [routerLink]="['/book', show.showId]">Select</a></div></article></div><p class="muted" *ngIf="!loading && !shows.length && !error">No shows are currently scheduled for this movie.</p></div></section></div>
<div class="loading page-wrap" *ngIf="!movie && !error">Loading movie…</div><div class="error-banner page-wrap" *ngIf="error && !movie">{{error}}</div>` })
export class MovieDetailPage implements OnInit {
  movie: Movie | null = null; shows: Show[] = []; loading = true; error = '';
  constructor(private route: ActivatedRoute, private api: ApiService) {}
  ngOnInit(): void { const id = Number(this.route.snapshot.paramMap.get('id')); this.api.movie(id).subscribe({ next: m => { this.movie = m; this.api.shows(id).subscribe({ next: s => { this.shows = s; this.loading = false; }, error: e => { this.error = e.error?.message || 'Could not load showtimes.'; this.loading = false; } }); }, error: e => { this.error = e.error?.message || 'Movie not found. Try signing in.'; this.loading = false; } }); }
}
