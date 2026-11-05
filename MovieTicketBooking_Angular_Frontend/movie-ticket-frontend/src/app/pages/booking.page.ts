import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiService } from '../core/api.service';
import { AuthService } from '../core/auth.service';
import { Show, ShowSeat, Booking } from '../core/models';

@Component({ selector: 'app-booking-page', standalone: true, imports: [CommonModule, FormsModule, RouterLink], template: `
<div class="page-wrap booking-page"><a class="back-link" routerLink="/">← Back to movies</a><div class="section-heading"><div><span class="eyebrow">MAKE IT A MOVIE NIGHT</span><h2>Select your <em>seats.</em></h2></div><div class="seat-legend"><span><i class="seat-dot available"></i> Available</span><span><i class="seat-dot selected"></i> Selected</span><span><i class="seat-dot booked"></i> Booked</span></div></div>
<div *ngIf="show" class="booking-layout"><section class="seat-panel"><div class="screen-visual"><span>SCREEN THIS WAY</span></div><div class="seat-grid" *ngIf="seats.length"><div class="seat-row" *ngFor="let row of rows"><span class="row-label">{{row}}</span><button *ngFor="let seat of seatsForRow(row)" class="seat" [class.booked]="isBooked(seat)" [class.selected]="isSelected(seat)" [disabled]="isBooked(seat)" (click)="toggle(seat)" [title]="row + seat.seatNumber + ' · ' + seat.seatType">{{seat.seatNumber}}</button></div></div><div *ngIf="!seats.length && !error" class="loading">Loading seat layout…</div><p class="seat-hint">Seat prices are based on the show's ticket price. Booked seats cannot be selected.</p></section>
<aside class="booking-summary"><span class="eyebrow">YOUR SELECTION</span><h3>{{show.movieTitle}}</h3><p>{{show.theatreName}} · {{show.screenName}}</p><p>{{show.startTime | date:'EEE, d MMM yyyy · h:mm a'}}</p><div class="summary-line"><span>Selected seats</span><b>{{selected.length}}</b></div><div class="selected-chips"><span *ngFor="let seat of selectedSeats">{{seat.rowName}}{{seat.seatNumber}}</span><small *ngIf="!selected.length">Choose seats to continue</small></div><div class="summary-total"><span>Total</span><b>{{selected.length * show.ticketPrice | currency:'INR':'symbol':'1.0-0'}}</b></div>
<form (ngSubmit)="book()"><label>Customer name<input name="customerName" [(ngModel)]="customerName" required maxlength="100"></label><label>Email<input name="customerEmail" [(ngModel)]="customerEmail" required type="email" maxlength="150"></label><label>Phone<input name="customerPhone" [(ngModel)]="customerPhone" required type="tel" maxlength="15"></label><div class="error-banner" *ngIf="error">{{error}}</div><button class="btn btn-primary full" [disabled]="busy || !selected.length">{{busy ? 'Confirming…' : 'Confirm booking'}} <span>→</span></button></form>
</aside></div>
<div *ngIf="confirmation" class="confirmation"><div class="confirm-icon">✓</div><span class="eyebrow">YOU'RE ALL SET</span><h2>It's a <em>movie night.</em></h2><p>Booking reference: <b>{{confirmation.bookingReference}}</b></p><p>{{confirmation.customerName}} · {{confirmation.seats?.length || selected.length}} seat(s) · {{confirmation.totalAmount | currency:'INR':'symbol':'1.0-0'}}</p><a routerLink="/" class="btn btn-primary">Back to home</a></div>
<div class="error-banner" *ngIf="!show && error">{{error}}</div></div>` })
export class BookingPage implements OnInit {
  show: Show | null = null; seats: ShowSeat[] = []; selected: number[] = []; customerName = ''; customerEmail = ''; customerPhone = ''; busy = false; error = ''; confirmation: Booking | null = null;
  constructor(private route: ActivatedRoute, private api: ApiService, private auth: AuthService, private router: Router) {}
  ngOnInit(): void { const id = Number(this.route.snapshot.paramMap.get('showId')); const user = this.auth.user(); if (!this.auth.isLoggedIn()) { this.router.navigate(['/login']); return; } if (user) { this.customerName = user.fullName; this.customerEmail = user.email; } this.api.show(id).subscribe({ next: s => { this.show = s; this.api.showSeats(id).subscribe({ next: list => this.seats = list, error: e => this.error = e.error?.message || 'Could not load seats.' }); }, error: e => this.error = e.error?.message || 'Show not found.' }); }
  get rows(): string[] { return [...new Set(this.seats.map(s => s.rowName))].sort(); }
  seatsForRow(row: string): ShowSeat[] { return this.seats.filter(s => s.rowName === row).sort((a,b) => a.seatNumber-b.seatNumber); }
  get selectedSeats(): ShowSeat[] { return this.seats.filter(s => this.selected.includes(s.seatId)); }
  isBooked(s: ShowSeat): boolean { return s.status.toLowerCase() !== 'available'; }
  isSelected(s: ShowSeat): boolean { return this.selected.includes(s.seatId); }
  toggle(s: ShowSeat): void { this.selected = this.isSelected(s) ? this.selected.filter(id => id !== s.seatId) : [...this.selected, s.seatId]; }
  book(): void { if (!this.show || !this.selected.length) return; this.busy = true; this.error = ''; this.api.createBooking({ showId: this.show.showId, customerName: this.customerName, customerEmail: this.customerEmail, customerPhone: this.customerPhone, seatIds: this.selected }).subscribe({ next: b => { this.confirmation = b; this.busy = false; }, error: e => { this.error = e.status === 403 ? 'Your backend currently allows only Admin to create bookings. Apply the customer booking authorization change in BACKEND_SETUP.md.' : e.error?.message || e.error?.errors?.join(', ') || 'Booking failed. Please refresh seats and try again.'; this.busy = false; } }); }
}
