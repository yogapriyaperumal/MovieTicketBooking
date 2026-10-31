using MovieTicketBooking.DTO.Movies;

namespace MovieTicketBooking.Services.Interfaces;

public interface IMovieService
{
    Task<PagedResponse<MovieResponse>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? title);

    Task<MovieResponse?> GetByIdAsync(int id);

    Task<MovieResponse> CreateAsync(
        CreateMovieRequest request);

    Task<MovieResponse?> UpdateAsync(
        int id,
        UpdateMovieRequest request);

    Task<MovieDeleteResult> DeleteAsync(int id);
}