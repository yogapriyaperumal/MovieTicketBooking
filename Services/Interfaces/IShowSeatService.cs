using MovieTicketBooking.DTOs.Shows;

namespace MovieTicketBooking.Services.Interfaces;

public interface IShowSeatService
{
    Task<(List<ShowSeatResponse>? Seats, string? Error)>
        GetByShowIdAsync(int showId);

    Task<(List<ShowSeatResponse>? Seats, string? Error)>
        GetAvailableByShowIdAsync(int showId);

    Task<(List<ShowSeatResponse>? Seats, string? Error)>
        GetBookedByShowIdAsync(int showId);

    Task<ShowSeatResponse?> GetByIdAsync(int showSeatId);
}