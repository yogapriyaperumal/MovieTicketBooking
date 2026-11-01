using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Respositories.Interfaces;

namespace MovieTicketBooking.Respositories;

public class ShowSeatRepository : IShowSeatRepository
{
    private readonly ApplicationDbContext _context;

    public ShowSeatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShowSeat>> GetByShowIdAsync(int showId)
    {
        return await _context.ShowSeats
            .AsNoTracking()
            .Include(ss => ss.Seat)
            .Where(ss => ss.ShowId == showId)
            .OrderBy(ss => ss.Seat!.RowName)
            .ThenBy(ss => ss.Seat!.SeatNumber)
            .ToListAsync();
    }

    public async Task<List<ShowSeat>> GetAvailableByShowIdAsync(int showId)
    {
        return await _context.ShowSeats
            .AsNoTracking()
            .Include(ss => ss.Seat)
            .Where(ss =>
                ss.ShowId == showId &&
                ss.Status == "Available")
            .OrderBy(ss => ss.Seat!.RowName)
            .ThenBy(ss => ss.Seat!.SeatNumber)
            .ToListAsync();
    }

    public async Task<List<ShowSeat>> GetBookedByShowIdAsync(int showId)
    {
        return await _context.ShowSeats
            .AsNoTracking()
            .Include(ss => ss.Seat)
            .Where(ss =>
                ss.ShowId == showId &&
                ss.Status == "Booked")
            .OrderBy(ss => ss.Seat!.RowName)
            .ThenBy(ss => ss.Seat!.SeatNumber)
            .ToListAsync();
    }

    public async Task<ShowSeat?> GetByIdAsync(int showSeatId)
    {
        return await _context.ShowSeats
            .AsNoTracking()
            .Include(ss => ss.Seat)
            .FirstOrDefaultAsync(ss =>
                ss.ShowSeatId == showSeatId);
    }

    public async Task<bool> ShowExistsAsync(int showId)
    {
        return await _context.Shows
            .AnyAsync(s => s.ShowId == showId);
    }
}