using MovieTicketBooking.DTOs.Shows;

namespace MovieTicketBooking.Services.Interfaces;

public interface IShowService
{
    Task<List<ShowResponse>> GetAllAsync(
        int? movieId,
        int? screenId);

    Task<ShowResponse?> GetByIdAsync(int showId);

    Task<(ShowResponse? Show, string? Error)> CreateAsync(
        CreateShowRequest request);

    Task<(bool Success, string? Error)> UpdateAsync(
        int showId,
        UpdateShowRequest request);

    Task<(bool Success, string? Error)> DeleteAsync(int showId);
}