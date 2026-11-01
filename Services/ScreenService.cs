using MovieTicketBooking.DTO.Screens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Respositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class ScreenService : IScreenService
{
    private readonly IScreenRepository _screenRepository;

    public ScreenService(IScreenRepository screenRepository)
    {
        _screenRepository = screenRepository;
    }

    // Get all screens or screens by theatre ID.
    public async Task<List<ScreenResponse>> GetAllAsync(int? theatreId)
    {
        var screens = await _screenRepository.GetAllAsync(theatreId);

        return screens.Select(MapToResponse).ToList();
    }

    // Get screen by ID.
    public async Task<ScreenResponse?> GetByIdAsync(int id)
    {
        var screen = await _screenRepository.GetByIdAsync(id);

        if (screen == null)
        {
            return null;
        }

        return MapToResponse(screen);
    }

    // Create screen.
    public async Task<ScreenCreateResult> CreateAsync(
        CreateScreenRequest request)
    {
        bool theatreExists =
            await _screenRepository.TheatreExistsAsync(request.TheatreId);

        if (!theatreExists)
        {
            return ScreenCreateResult.TheatreNotFound;
        }

        string screenName = request.ScreenName.Trim();

        bool duplicate = await _screenRepository.ScreenNameExistsAsync(
            request.TheatreId,
            screenName);

        if (duplicate)
        {
            return ScreenCreateResult.DuplicateName;
        }

        var screen = new Screen
        {
            TheatreId = request.TheatreId,
            ScreenName = screenName
        };

        await _screenRepository.CreateAsync(screen);

        return ScreenCreateResult.Created;
    }

    // Update screen.
    public async Task<ScreenUpdateResult> UpdateAsync(
        int id,
        UpdateScreenRequest request)
    {
        var screen = await _screenRepository.GetByIdAsync(id);

        if (screen == null)
        {
            return ScreenUpdateResult.ScreenNotFound;
        }

        bool theatreExists =
            await _screenRepository.TheatreExistsAsync(request.TheatreId);

        if (!theatreExists)
        {
            return ScreenUpdateResult.TheatreNotFound;
        }

        string screenName = request.ScreenName.Trim();

        bool duplicate = await _screenRepository.ScreenNameExistsAsync(
            request.TheatreId,
            screenName,
            id);

        if (duplicate)
        {
            return ScreenUpdateResult.DuplicateName;
        }

        screen.TheatreId = request.TheatreId;
        screen.ScreenName = screenName;

        await _screenRepository.UpdateAsync(screen);

        return ScreenUpdateResult.Updated;
    }

    // Delete screen.
    public async Task<ScreenDeleteResult> DeleteAsync(int id)
    {
        var screen = await _screenRepository.GetByIdAsync(id);

        if (screen == null)
        {
            return ScreenDeleteResult.ScreenNotFound;
        }

        bool hasDependencies =
            await _screenRepository.HasSeatsOrShowsAsync(id);

        if (hasDependencies)
        {
            return ScreenDeleteResult.HasDependencies;
        }

        await _screenRepository.DeleteAsync(screen);

        return ScreenDeleteResult.Deleted;
    }

    // Convert entity into response DTO.
    private static ScreenResponse MapToResponse(Screen screen)
    {
        return new ScreenResponse
        {
            ScreenId = screen.ScreenId,
            TheatreId = screen.TheatreId,
            TheatreName = screen.Theatre?.Name ?? string.Empty,
            ScreenName = screen.ScreenName
        };
    }
}