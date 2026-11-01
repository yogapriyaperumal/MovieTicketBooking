using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class SeatRepository : ISeatRepository
{
    private readonly ApplicationDbContext _context;

    public SeatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Seat>> GetAllAsync(int? screenId)
    {
        var query = _context.Seats
            .AsNoTracking()
            .Include(s => s.Screen)
            .AsQueryable();

        if (screenId.HasValue)
        {
            query = query.Where(s => s.ScreenId == screenId.Value);
        }

        return await query
            .OrderBy(s => s.ScreenId)
            .ThenBy(s => s.RowName)
            .ThenBy(s => s.SeatNumber)
            .ToListAsync();
    }

    public async Task<Seat?> GetByIdAsync(int seatId)
    {
        return await _context.Seats
            .AsNoTracking()
            .Include(s => s.Screen)
            .FirstOrDefaultAsync(s => s.SeatId == seatId);
    }

    public async Task<bool> ScreenExistsAsync(int screenId)
    {
        return await _context.Screens
            .AnyAsync(s => s.ScreenId == screenId);
    }

    public async Task<bool> SeatExistsAsync(
        int screenId,
        string rowName,
        int seatNumber,
        int? excludeSeatId = null)
    {
        var query = _context.Seats.Where(s =>
            s.ScreenId == screenId &&
            s.RowName == rowName &&
            s.SeatNumber == seatNumber);

        if (excludeSeatId.HasValue)
        {
            query = query.Where(s => s.SeatId != excludeSeatId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Seat> CreateAsync(Seat seat)
    {
        await _context.Seats.AddAsync(seat);
        await _context.SaveChangesAsync();

        return seat;
    }

    public async Task<List<Seat>> BulkCreateAsync(List<Seat> seats)
    {
        await _context.Seats.AddRangeAsync(seats);
        await _context.SaveChangesAsync();

        return seats;
    }

    public async Task UpdateAsync(Seat seat)
    {
        _context.Seats.Attach(seat);

        _context.Entry(seat)
            .Property(s => s.RowName).IsModified = true;

        _context.Entry(seat)
            .Property(s => s.SeatNumber).IsModified = true;

        _context.Entry(seat)
            .Property(s => s.SeatType).IsModified = true;

        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasDependenciesAsync(int seatId)
    {
        return await _context.ShowSeats
                   .AnyAsync(ss => ss.SeatId == seatId)
               || await _context.BookingSeats
                   .AnyAsync(bs => bs.SeatId == seatId);
    }

    public async Task DeleteAsync(int seatId)
    {
        var seat = await _context.Seats.FindAsync(seatId);

        if (seat == null)
        {
            return;
        }

        _context.Seats.Remove(seat);
        await _context.SaveChangesAsync();
    }
}