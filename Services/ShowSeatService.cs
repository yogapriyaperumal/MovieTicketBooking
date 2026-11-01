using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Respositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class ShowSeatService : IShowSeatService
{
    private readonly IShowSeatRepository _repository;

    public ShowSeatService(IShowSeatRepository repository)
    {
        _repository = repository;
    }

    private static ShowSeatResponse MapToResponse(ShowSeat showSeat)
    {
        return new ShowSeatResponse
        {
            ShowSeatId = showSeat.ShowSeatId,
            ShowId = showSeat.ShowId,
            SeatId = showSeat.SeatId,
            RowName = showSeat.Seat?.RowName ?? string.Empty,
            SeatNumber = showSeat.Seat?.SeatNumber ?? 0,
            SeatType = showSeat.Seat?.SeatType ?? string.Empty,
            Status = showSeat.Status
        };
    }

    public async Task<(List<ShowSeatResponse>? Seats, string? Error)>
        GetByShowIdAsync(int showId)
    {
        if (!await _repository.ShowExistsAsync(showId))
        {
            return (null, "Show not found.");
        }

        var seats = await _repository.GetByShowIdAsync(showId);

        return (seats.Select(MapToResponse).ToList(), null);
    }

    public async Task<(List<ShowSeatResponse>? Seats, string? Error)>
        GetAvailableByShowIdAsync(int showId)
    {
        if (!await _repository.ShowExistsAsync(showId))
        {
            return (null, "Show not found.");
        }

        var seats = await _repository.GetAvailableByShowIdAsync(showId);

        return (seats.Select(MapToResponse).ToList(), null);
    }

    public async Task<(List<ShowSeatResponse>? Seats, string? Error)>
        GetBookedByShowIdAsync(int showId)
    {
        if (!await _repository.ShowExistsAsync(showId))
        {
            return (null, "Show not found.");
        }

        var seats = await _repository.GetBookedByShowIdAsync(showId);

        return (seats.Select(MapToResponse).ToList(), null);
    }

    public async Task<ShowSeatResponse?> GetByIdAsync(int showSeatId)
    {
        var showSeat = await _repository.GetByIdAsync(showSeatId);

        return showSeat == null ? null : MapToResponse(showSeat);
    }
}