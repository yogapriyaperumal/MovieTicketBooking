import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Booking, Movie, Paged, Screen, Show, ShowSeat, Theatre } from './models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly base = environment.apiUrl;
  constructor(private http: HttpClient) {}

  movies(title = '', pageNumber = 1, pageSize = 30): Observable<Paged<Movie>> {
    let params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);
    if (title.trim()) params = params.set('title', title.trim());
    // The backend protects /movies but exposes title search publicly.
    if (title.trim()) return this.http.get<Paged<Movie>>(`${this.base}/movies/search`, { params });
    return this.http.get<Paged<Movie>>(`${this.base}/movies`, { params });
  }
  searchMovies(title: string): Observable<Paged<Movie>> {
    return this.movies(title, 1, 30);
  }
  movie(id: number): Observable<Movie> { return this.http.get<Movie>(`${this.base}/movies/${id}`); }
  theatres(): Observable<Theatre[]> { return this.http.get<Theatre[]>(`${this.base}/theatres`); }
  screens(theatreId?: number): Observable<Screen[]> {
    const params = theatreId ? new HttpParams().set('theatreId', theatreId) : undefined;
    return this.http.get<Screen[]>(`${this.base}/screens`, { params });
  }
  shows(movieId?: number): Observable<Show[]> {
    const params = movieId ? new HttpParams().set('movieId', movieId) : undefined;
    return this.http.get<Show[]>(`${this.base}/shows`, { params });
  }
  show(id: number): Observable<Show> { return this.http.get<Show>(`${this.base}/shows/${id}`); }
  showSeats(showId: number): Observable<ShowSeat[]> { return this.http.get<ShowSeat[]>(`${this.base}/show-seats/show/${showId}`); }
  createBooking(payload: { showId: number; customerName: string; customerEmail: string; customerPhone: string; seatIds: number[] }): Observable<Booking> {
    return this.http.post<Booking>(`${this.base}/bookings`, payload);
  }
  createMovie(payload: Partial<Movie>): Observable<Movie> { return this.http.post<Movie>(`${this.base}/movies`, payload); }
  createTheatre(payload: { name: string; location: string }): Observable<Theatre> { return this.http.post<Theatre>(`${this.base}/theatres`, payload); }
  createShow(payload: { movieId: number; screenId: number; startTime: string; endTime: string; ticketPrice: number }): Observable<Show> {
    return this.http.post<Show>(`${this.base}/shows`, payload);
  }
  createScreen(payload: { theatreId: number; screenName: string }): Observable<unknown> {
    return this.http.post(`${this.base}/screens`, payload);
  }
  bulkCreateSeats(payload: { screenId: number; seats: { rowName: string; seatNumber: number; seatType: string }[] }): Observable<unknown> {
    return this.http.post(`${this.base}/seats/bulk`, payload);
  }
  bookings(): Observable<Booking[]> { return this.http.get<Booking[]>(`${this.base}/bookings`); }
  cancelBooking(id: number): Observable<unknown> { return this.http.put(`${this.base}/bookings/${id}/cancel`, {}); }
}
