using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Respositories.Interfaces;

namespace MovieTicketBooking.Respositories;

public class ShowRepository : IShowRepository
{
    private readonly ApplicationDbContext _context;

    public ShowRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Show>> GetAllAsync(
        int? movieId,
        int? screenId)
    {
        var query = _context.Shows
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Screen)
                .ThenInclude(sc => sc!.Theatre)
            .AsQueryable();

        if (movieId.HasValue)
        {
            query = query.Where(s => s.MovieId == movieId.Value);
        }

        if (screenId.HasValue)
        {
            query = query.Where(s => s.ScreenId == screenId.Value);
        }

        return await query
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<Show?> GetByIdAsync(int showId)
    {
        return await _context.Shows
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Screen)
                .ThenInclude(sc => sc!.Theatre)
            .FirstOrDefaultAsync(s => s.ShowId == showId);
    }

    public async Task<bool> MovieExistsAsync(int movieId)
    {
        return await _context.Movies
            .AnyAsync(m => m.MovieId == movieId);
    }

    public async Task<bool> ScreenExistsAsync(int screenId)
    {
        return await _context.Screens
            .AnyAsync(s => s.ScreenId == screenId);
    }

    public async Task<bool> HasOverlapAsync(
        int screenId,
        DateTime startTime,
        DateTime endTime,
        int? excludeShowId = null)
    {
        var query = _context.Shows.Where(s =>
            s.ScreenId == screenId &&
            startTime < s.EndTime &&
            endTime > s.StartTime);

        if (excludeShowId.HasValue)
        {
            query = query.Where(s =>
                s.ShowId != excludeShowId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Show> CreateAsync(Show show)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        await _context.Shows.AddAsync(show);

        await _context.SaveChangesAsync();

        // Get all physical seats belonging to this screen.
        var seats = await _context.Seats
            .Where(s => s.ScreenId == show.ScreenId)
            .ToListAsync();

        // Create one Available ShowSeat for every physical seat.
        var showSeats = seats.Select(seat => new ShowSeat
        {
            ShowId = show.ShowId,
            SeatId = seat.SeatId,
            Status = "Available"
        }).ToList();

        await _context.ShowSeats.AddRangeAsync(showSeats);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return show;
    }

    public async Task UpdateAsync(Show show)
    {
        var existingShow = await _context.Shows
            .FirstOrDefaultAsync(s => s.ShowId == show.ShowId);

        if (existingShow == null)
        {
            return;
        }

        existingShow.MovieId = show.MovieId;
        existingShow.ScreenId = show.ScreenId;
        existingShow.StartTime = show.StartTime;
        existingShow.EndTime = show.EndTime;
        existingShow.TicketPrice = show.TicketPrice;

        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasBookingsAsync(int showId)
    {
        return await _context.Bookings
            .AnyAsync(b => b.ShowId == showId);
    }

    public async Task DeleteAsync(int showId)
    {
        var show = await _context.Shows
            .FirstOrDefaultAsync(s => s.ShowId == showId);

        if (show == null)
        {
            return;
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        // Remove associated ShowSeats before deleting the show.
        var showSeats = await _context.ShowSeats
            .Where(ss => ss.ShowId == showId)
            .ToListAsync();

        _context.ShowSeats.RemoveRange(showSeats);

        _context.Shows.Remove(show);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
    }
}