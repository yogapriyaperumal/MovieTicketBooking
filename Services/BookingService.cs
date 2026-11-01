using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Respositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;

    public BookingService(IBookingRepository repository)
    {
        _repository = repository;
    }

    private static BookingResponse MapToResponse(Booking booking)
    {
        return new BookingResponse
        {
            BookingId = booking.BookingId,
            BookingReference = booking.BookingReference,
            ShowId = booking.ShowId,
            MovieTitle = booking.Show?.Movie?.Title ?? string.Empty,
            ScreenName = booking.Show?.Screen?.ScreenName ?? string.Empty,
            CustomerName = booking.CustomerName,
            CustomerEmail = booking.CustomerEmail,
            CustomerPhone = booking.CustomerPhone,
            BookingDate = booking.BookingDate,
            TotalAmount = booking.TotalAmount,
            Status = booking.Status,

            Seats = booking.BookingSeats.Select(bs => new BookingSeatResponse
            {
                SeatId = bs.SeatId,
                RowName = bs.Seat?.RowName ?? string.Empty,
                SeatNumber = bs.Seat?.SeatNumber ?? 0,
                SeatType = bs.Seat?.SeatType ?? string.Empty,
                Price = bs.Price
            }).ToList()
        };
    }

    public async Task<List<BookingResponse>> GetAllAsync()
    {
        var bookings = await _repository.GetAllAsync();

        return bookings.Select(MapToResponse).ToList();
    }

    public async Task<BookingResponse?> GetByIdAsync(int bookingId)
    {
        var booking = await _repository.GetByIdAsync(bookingId);

        return booking == null ? null : MapToResponse(booking);
    }

    public async Task<BookingResponse?> GetByReferenceAsync(
        string bookingReference)
    {
        var booking = await _repository.GetByReferenceAsync(
            bookingReference);

        return booking == null ? null : MapToResponse(booking);
    }

    public async Task<(BookingResponse? Booking, string? Error)> CreateAsync(
        CreateBookingRequest request)
    {
        if (request.SeatIds == null || request.SeatIds.Count == 0)
        {
            return (null, "Select at least one seat.");
        }

        if (request.SeatIds.Any(id => id <= 0))
        {
            return (null, "Seat IDs must be greater than zero.");
        }

        if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
        {
            return (null, "Duplicate seat IDs are not allowed.");
        }

        var booking = new Booking
        {
            ShowId = request.ShowId,
            CustomerName = request.CustomerName.Trim(),
            CustomerEmail = request.CustomerEmail.Trim(),
            CustomerPhone = request.CustomerPhone.Trim()
        };

        var (createdBooking, error) = await _repository.CreateAsync(
            booking,
            request.SeatIds);

        if (error != null || createdBooking == null)
        {
            return (null, error ?? "Unable to create booking.");
        }

        // Reload navigation properties for the response.
        var result = await _repository.GetByIdAsync(
            createdBooking.BookingId);

        return (result == null ? null : MapToResponse(result), null);
    }

    public async Task<(CancelBookingResponse? Response, string? Error)>
        CancelAsync(int bookingId)
    {
        var (success, error) = await _repository.CancelAsync(bookingId);

        if (!success)
        {
            return (null, error);
        }

        var booking = await _repository.GetByIdAsync(bookingId);

        if (booking == null)
        {
            return (null, "Booking not found.");
        }

        var response = new CancelBookingResponse
        {
            BookingId = booking.BookingId,
            BookingReference = booking.BookingReference,
            Status = booking.Status,
            Message = "Booking cancelled and seats released successfully."
        };

        return (response, null);
    }
}