using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Respositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class ShowService : IShowService
{
    private readonly IShowRepository _repository;

    public ShowService(IShowRepository repository)
    {
        _repository = repository;
    }

    private static ShowResponse MapToResponse(Show show)
    {
        return new ShowResponse
        {
            ShowId = show.ShowId,
            MovieId = show.MovieId,
            MovieTitle = show.Movie?.Title ?? string.Empty,
            ScreenId = show.ScreenId,
            ScreenName = show.Screen?.ScreenName ?? string.Empty,
            TheatreName = show.Screen?.Theatre?.Name ?? string.Empty,
            StartTime = show.StartTime,
            EndTime = show.EndTime,
            TicketPrice = show.TicketPrice
        };
    }

    public async Task<List<ShowResponse>> GetAllAsync(
        int? movieId,
        int? screenId)
    {
        var shows = await _repository.GetAllAsync(movieId, screenId);

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<ShowResponse?> GetByIdAsync(int showId)
    {
        var show = await _repository.GetByIdAsync(showId);

        return show == null ? null : MapToResponse(show);
    }

    public async Task<(ShowResponse? Show, string? Error)> CreateAsync(
        CreateShowRequest request)
    {
        if (request.EndTime <= request.StartTime)
        {
            return (null, "End time must be later than start time.");
        }

        if (request.TicketPrice < 0)
        {
            return (null, "Ticket price cannot be negative.");
        }

        if (!await _repository.MovieExistsAsync(request.MovieId))
        {
            return (null, "Movie not found.");
        }

        if (!await _repository.ScreenExistsAsync(request.ScreenId))
        {
            return (null, "Screen not found.");
        }

        var overlap = await _repository.HasOverlapAsync(
            request.ScreenId,
            request.StartTime,
            request.EndTime);

        if (overlap)
        {
            return (null, "Another show is already scheduled on this screen during this time.");
        }

        var show = new Show
        {
            MovieId = request.MovieId,
            ScreenId = request.ScreenId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TicketPrice = request.TicketPrice
        };

        var createdShow = await _repository.CreateAsync(show);

        var response = await _repository.GetByIdAsync(
            createdShow.ShowId);

        return (response == null ? null : MapToResponse(response), null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(
        int showId,
        UpdateShowRequest request)
    {
        var existingShow = await _repository.GetByIdAsync(showId);

        if (existingShow == null)
        {
            return (false, "Show not found.");
        }

        if (request.EndTime <= request.StartTime)
        {
            return (false, "End time must be later than start time.");
        }

        if (request.TicketPrice < 0)
        {
            return (false, "Ticket price cannot be negative.");
        }

        if (!await _repository.MovieExistsAsync(request.MovieId))
        {
            return (false, "Movie not found.");
        }

        if (!await _repository.ScreenExistsAsync(request.ScreenId))
        {
            return (false, "Screen not found.");
        }

        var overlap = await _repository.HasOverlapAsync(
            request.ScreenId,
            request.StartTime,
            request.EndTime,
            showId);

        if (overlap)
        {
            return (false, "Another show is already scheduled on this screen during this time.");
        }

        var show = new Show
        {
            ShowId = showId,
            MovieId = request.MovieId,
            ScreenId = request.ScreenId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TicketPrice = request.TicketPrice
        };

        await _repository.UpdateAsync(show);

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int showId)
    {
        var existingShow = await _repository.GetByIdAsync(showId);

        if (existingShow == null)
        {
            return (false, "Show not found.");
        }

        if (await _repository.HasBookingsAsync(showId))
        {
            return (false, "Cannot delete a show that has bookings.");
        }

        await _repository.DeleteAsync(showId);

        return (true, null);
    }
}