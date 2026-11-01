using System.Data;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Respositories.Interfaces;

namespace MovieTicketBooking.Respositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Show)
                .ThenInclude(s => s!.Movie)
            .Include(b => b.Show)
                .ThenInclude(s => s!.Screen)
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(int bookingId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Show)
                .ThenInclude(s => s!.Movie)
            .Include(b => b.Show)
                .ThenInclude(s => s!.Screen)
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);
    }

    public async Task<Booking?> GetByReferenceAsync(string bookingReference)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Show)
                .ThenInclude(s => s!.Movie)
            .Include(b => b.Show)
                .ThenInclude(s => s!.Screen)
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .FirstOrDefaultAsync(b =>
                b.BookingReference == bookingReference);
    }

    public async Task<(Booking? Booking, string? Error)> CreateAsync(
        Booking booking,
        List<int> seatIds)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var show = await _context.Shows
            .FirstOrDefaultAsync(s => s.ShowId == booking.ShowId);

        if (show == null)
        {
            return (null, "Show not found.");
        }

        // Retrieve the requested ShowSeats for this show.
        var showSeats = await _context.ShowSeats
            .Include(ss => ss.Seat)
            .Where(ss =>
                ss.ShowId == booking.ShowId &&
                seatIds.Contains(ss.SeatId))
            .ToListAsync();

        // Every requested seat must belong to this show.
        if (showSeats.Count != seatIds.Count)
        {
            return (null, "One or more selected seats do not belong to this show.");
        }

        // Every requested seat must still be available.
        if (showSeats.Any(ss => ss.Status != "Available"))
        {
            return (null, "One or more selected seats are already booked.");
        }

        booking.BookingReference =
            $"MTB-{Guid.NewGuid():N}".ToUpperInvariant()[..30];

        booking.BookingDate = DateTime.UtcNow;
        booking.Status = "Confirmed";

        booking.TotalAmount = show.TicketPrice * seatIds.Count;

        booking.BookingSeats = showSeats.Select(ss => new BookingSeat
        {
            SeatId = ss.SeatId,
            Price = show.TicketPrice
        }).ToList();

        // Mark selected ShowSeats as booked.
        foreach (var showSeat in showSeats)
        {
            showSeat.Status = "Booked";
        }

        await _context.Bookings.AddAsync(booking);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return (booking, null);
    }

    public async Task<(bool Success, string? Error)> CancelAsync(int bookingId)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var booking = await _context.Bookings
            .Include(b => b.BookingSeats)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);

        if (booking == null)
        {
            return (false, "Booking not found.");
        }

        if (booking.Status != "Confirmed")
        {
            return (false, "Only confirmed bookings can be cancelled.");
        }

        var seatIds = booking.BookingSeats
            .Select(bs => bs.SeatId)
            .ToList();

        var showSeats = await _context.ShowSeats
            .Where(ss =>
                ss.ShowId == booking.ShowId &&
                seatIds.Contains(ss.SeatId))
            .ToListAsync();

        booking.Status = "Cancelled";

        foreach (var showSeat in showSeats)
        {
            showSeat.Status = "Available";
        }

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return (true, null);
    }
}