using MovieTicketBooking.DTOs.Seats;

namespace MovieTicketBooking.Services.Interfaces;

public interface ISeatService
{
    Task<List<SeatResponse>> GetAllAsync(int? screenId);

    Task<SeatResponse?> GetByIdAsync(int seatId);

    Task<(SeatResponse? Seat, string? Error)> CreateAsync(
        CreateSeatRequest request);

    Task<(List<SeatResponse>? Seats, string? Error)> BulkCreateAsync(
        BulkCreateSeatsRequest request);

    Task<(bool Success, string? Error)> UpdateAsync(
        int seatId,
        UpdateSeatRequest request);

    Task<(bool Success, string? Error)> DeleteAsync(int seatId);
}