using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class SeatService : ISeatService
{
    private readonly ISeatRepository _repository;

    public SeatService(ISeatRepository repository)
    {
        _repository = repository;
    }

    private static SeatResponse MapToResponse(Seat seat)
    {
        return new SeatResponse
        {
            SeatId = seat.SeatId,
            ScreenId = seat.ScreenId,
            ScreenName = seat.Screen?.ScreenName ?? string.Empty,
            RowName = seat.RowName,
            SeatNumber = seat.SeatNumber,
            SeatType = seat.SeatType
        };
    }

    public async Task<List<SeatResponse>> GetAllAsync(int? screenId)
    {
        var seats = await _repository.GetAllAsync(screenId);

        return seats.Select(MapToResponse).ToList();
    }

    public async Task<SeatResponse?> GetByIdAsync(int seatId)
    {
        var seat = await _repository.GetByIdAsync(seatId);

        return seat == null ? null : MapToResponse(seat);
    }

    public async Task<(SeatResponse? Seat, string? Error)> CreateAsync(
        CreateSeatRequest request)
    {
        if (!await _repository.ScreenExistsAsync(request.ScreenId))
        {
            return (null, "Screen not found.");
        }

        var duplicate = await _repository.SeatExistsAsync(
            request.ScreenId,
            request.RowName.Trim(),
            request.SeatNumber);

        if (duplicate)
        {
            return (null, "This seat already exists in the screen.");
        }

        var seat = new Seat
        {
            ScreenId = request.ScreenId,
            RowName = request.RowName.Trim(),
            SeatNumber = request.SeatNumber,
            SeatType = request.SeatType.Trim()
        };

        var createdSeat = await _repository.CreateAsync(seat);

        var response = await _repository.GetByIdAsync(createdSeat.SeatId);

        return (response == null ? null : MapToResponse(response), null);
    }

    public async Task<(List<SeatResponse>? Seats, string? Error)> BulkCreateAsync(
        BulkCreateSeatsRequest request)
    {
        if (!await _repository.ScreenExistsAsync(request.ScreenId))
        {
            return (null, "Screen not found.");
        }

        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in request.Seats)
        {
            var rowName = item.RowName.Trim();

            var key = $"{rowName}|{item.SeatNumber}";

            if (!keys.Add(key))
            {
                return (null, $"Duplicate seat in request: {key}");
            }

            var exists = await _repository.SeatExistsAsync(
                request.ScreenId,
                rowName,
                item.SeatNumber);

            if (exists)
            {
                return (null, $"Seat already exists: {key}");
            }
        }

        var seats = request.Seats.Select(item => new Seat
        {
            ScreenId = request.ScreenId,
            RowName = item.RowName.Trim(),
            SeatNumber = item.SeatNumber,
            SeatType = item.SeatType.Trim()
        }).ToList();

        var createdSeats = await _repository.BulkCreateAsync(seats);

        var createdIds = createdSeats
     .Select(s => s.SeatId)
     .ToHashSet();

        var allScreenSeats = await _repository.GetAllAsync(request.ScreenId);

        var responses = allScreenSeats
            .Where(s => createdIds.Contains(s.SeatId))
            .Select(MapToResponse)
            .ToList();

        return (responses, null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(
        int seatId,
        UpdateSeatRequest request)
    {
        var existingSeat = await _repository.GetByIdAsync(seatId);

        if (existingSeat == null)
        {
            return (false, "Seat not found.");
        }

        var duplicate = await _repository.SeatExistsAsync(
            existingSeat.ScreenId,
            request.RowName.Trim(),
            request.SeatNumber,
            seatId);

        if (duplicate)
        {
            return (false, "Another seat with the same row and number already exists.");
        }

        var seat = new Seat
        {
            SeatId = seatId,
            ScreenId = existingSeat.ScreenId,
            RowName = request.RowName.Trim(),
            SeatNumber = request.SeatNumber,
            SeatType = request.SeatType.Trim()
        };

        await _repository.UpdateAsync(seat);

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int seatId)
    {
        var existingSeat = await _repository.GetByIdAsync(seatId);

        if (existingSeat == null)
        {
            return (false, "Seat not found.");
        }

        if (await _repository.HasDependenciesAsync(seatId))
        {
            return (false, "Cannot delete this seat because it is associated with shows or bookings.");
        }

        await _repository.DeleteAsync(seatId);

        return (true, null);
    }
}