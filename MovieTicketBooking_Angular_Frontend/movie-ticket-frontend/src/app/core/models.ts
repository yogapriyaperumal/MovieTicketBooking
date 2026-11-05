export interface Movie {
  movieId: number; title: string; description?: string | null; durationMinutes: number;
  releaseDate: string; language: string; genre: string;
}
export interface Paged<T> { items: T[]; pageNumber: number; pageSize: number; totalRecords: number; totalPages: number; }
export interface Theatre { theatreId: number; name: string; location: string; }
export interface Screen { screenId: number; theatreId: number; theatreName: string; screenName: string; }
export interface Show {
  showId: number; movieId: number; movieTitle: string; screenId: number; screenName: string;
  theatreName: string; startTime: string; endTime: string; ticketPrice: number;
}
export interface ShowSeat {
  showSeatId: number; showId: number; seatId: number; rowName: string; seatNumber: number;
  seatType: string; status: string;
}
export interface AuthUser {
  userId: string; fullName: string; email: string; roles: string[];
  accessToken: string; expiresAt: string; tokenType: string;
}
export interface Booking {
  bookingId: number; bookingReference: string; customerName: string; customerEmail: string;
  customerPhone: string; bookingDate: string; totalAmount: number; status: string;
  seats: { seatId: number; rowName: string; seatNumber: number; seatType: string; price: number }[];
}
