using MovieTicketBooking.Models;

namespace MovieTicketBooking.Respositories.Interfaces
{
    public class IMovieRepository
    {
        Task<List<Movie>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? title);

        Task<int> GetCountAsync(string? title);

        Task<Movie?> GetByIdAsync(int id);

        Task<Movie> CreateAsync(Movie movie);

        Task UpdateAsync(Movie movie);

        Task<bool> HasShowsAsync(int movieId);

        Task DeleteAsync(Movie movie);
    }
}
