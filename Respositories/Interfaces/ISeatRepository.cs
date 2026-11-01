using MovieTicketBooking.Models;

namespace MovieTicketBooking.Respositories.Interfaces;

public interface ISeatRepository
{
    Task<List<Seat>> GetAllAsync(int? screenId);

    Task<Seat?> GetByIdAsync(int seatId);

    Task<bool> ScreenExistsAsync(int screenId);

    Task<bool> SeatExistsAsync(
        int screenId,
        string rowName,
        int seatNumber,
        int? excludeSeatId = null);

    Task<Seat> CreateAsync(Seat seat);

    Task<List<Seat>> BulkCreateAsync(List<Seat> seats);

    Task UpdateAsync(Seat seat);

    Task<bool> HasDependenciesAsync(int seatId);

    Task DeleteAsync(int seatId);
}