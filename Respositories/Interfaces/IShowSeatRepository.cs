using MovieTicketBooking.Models;

namespace MovieTicketBooking.Respositories.Interfaces;

public interface IShowSeatRepository
{
    Task<List<ShowSeat>> GetByShowIdAsync(int showId);

    Task<List<ShowSeat>> GetAvailableByShowIdAsync(int showId);

    Task<List<ShowSeat>> GetBookedByShowIdAsync(int showId);

    Task<ShowSeat?> GetByIdAsync(int showSeatId);

    Task<bool> ShowExistsAsync(int showId);
}