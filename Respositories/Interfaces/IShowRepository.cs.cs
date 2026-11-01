using MovieTicketBooking.Models;

namespace MovieTicketBooking.Respositories.Interfaces;

public interface IShowRepository
{
    Task<List<Show>> GetAllAsync(
        int? movieId,
        int? screenId);

    Task<Show?> GetByIdAsync(int showId);

    Task<bool> MovieExistsAsync(int movieId);

    Task<bool> ScreenExistsAsync(int screenId);

    Task<bool> HasOverlapAsync(
        int screenId,
        DateTime startTime,
        DateTime endTime,
        int? excludeShowId = null);

    Task<Show> CreateAsync(Show show);

    Task UpdateAsync(Show show);

    Task<bool> HasBookingsAsync(int showId);

    Task DeleteAsync(int showId);
}