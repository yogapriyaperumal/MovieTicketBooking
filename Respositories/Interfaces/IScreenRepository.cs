using MovieTicketBooking.Models;

namespace MovieTicketBooking.Respositories.Interfaces;

public interface IScreenRepository
{
    Task<List<Screen>> GetAllAsync(int? theatreId);

    Task<Screen?> GetByIdAsync(int id);

    Task<Screen> CreateAsync(Screen screen);

    Task UpdateAsync(Screen screen);

    Task<bool> TheatreExistsAsync(int theatreId);

    Task<bool> ScreenNameExistsAsync(
        int theatreId,
        string screenName,
        int? excludeScreenId = null);

    Task<bool> HasSeatsOrShowsAsync(int screenId);

    Task DeleteAsync(Screen screen);
}