using MovieTicketBooking.Models;

namespace MovieTicketBooking.Respositories.Interfaces;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync();

    Task<Booking?> GetByIdAsync(int bookingId);

    Task<Booking?> GetByReferenceAsync(string bookingReference);

    Task<(Booking? Booking, string? Error)> CreateAsync(
        Booking booking,
        List<int> seatIds);

    Task<(bool Success, string? Error)> CancelAsync(int bookingId);
}