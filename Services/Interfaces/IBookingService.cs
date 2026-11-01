using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces;

public interface IBookingService
{
    Task<List<BookingResponse>> GetAllAsync();

    Task<BookingResponse?> GetByIdAsync(int bookingId);

    Task<BookingResponse?> GetByReferenceAsync(string bookingReference);

    Task<(BookingResponse? Booking, string? Error)> CreateAsync(
        CreateBookingRequest request);

    Task<(CancelBookingResponse? Response, string? Error)> CancelAsync(
        int bookingId);
}