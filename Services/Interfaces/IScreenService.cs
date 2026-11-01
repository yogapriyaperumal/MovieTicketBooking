using MovieTicketBooking.DTO.Screens;

namespace MovieTicketBooking.Services.Interfaces;

public interface IScreenService
{
    Task<List<ScreenResponse>> GetAllAsync(int? theatreId);

    Task<ScreenResponse?> GetByIdAsync(int id);

    Task<ScreenCreateResult> CreateAsync(
        CreateScreenRequest request);

    Task<ScreenUpdateResult> UpdateAsync(
        int id,
        UpdateScreenRequest request);

    Task<ScreenDeleteResult> DeleteAsync(int id);
}