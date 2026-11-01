using MovieTicketBooking.Models;

namespace MovieTicketBooking.Respositories.Interfaces
{
    public interface ITheatreRepository
    {
        Task<List<Theatre>> GetAllAsync();

        Task<Theatre?> GetByIdAsync(int id);

        Task<Theatre> CreateAsync(Theatre theatre);

        Task UpdateAsync(Theatre theatre);

        Task<bool> HasScreensAsync(int theatreId);

        Task DeleteAsync(Theatre theatre);
    }
}
