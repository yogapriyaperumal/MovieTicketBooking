using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Respositories.Interfaces;

namespace MovieTicketBooking.Respositories;

public class ScreenRepository : IScreenRepository
{
    private readonly ApplicationDbContext _context;

    public ScreenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Get all screens or filter by theatre.
    public async Task<List<Screen>> GetAllAsync(int? theatreId)
    {
        IQueryable<Screen> query = _context.Screens
            .AsNoTracking()
            .Include(s => s.Theatre);

        if (theatreId.HasValue)
        {
            query = query.Where(s => s.TheatreId == theatreId.Value);
        }

        return await query
            .OrderBy(s => s.ScreenId)
            .ToListAsync();
    }

    // Get screen by ID.
    public async Task<Screen?> GetByIdAsync(int id)
    {
        return await _context.Screens
            .AsNoTracking()
            .Include(s => s.Theatre)
            .FirstOrDefaultAsync(s => s.ScreenId == id);
    }

    // Create screen.
    public async Task<Screen> CreateAsync(Screen screen)
    {
        await _context.Screens.AddAsync(screen);

        await _context.SaveChangesAsync();

        return screen;
    }

    // Update screen.
    public async Task UpdateAsync(Screen screen)
    {
        _context.Attach(screen);

        _context.Entry(screen)
            .Property(s => s.TheatreId).IsModified = true;

        _context.Entry(screen)
            .Property(s => s.ScreenName).IsModified = true;

        await _context.SaveChangesAsync();
    }

    // Check whether theatre exists.
    public async Task<bool> TheatreExistsAsync(int theatreId)
    {
        return await _context.Theatres
            .AnyAsync(t => t.TheatreId == theatreId);
    }

    // Prevent duplicate screen names within the same theatre.
    public async Task<bool> ScreenNameExistsAsync(
        int theatreId,
        string screenName,
        int? excludeScreenId = null)
    {
        var query = _context.Screens.Where(s =>
            s.TheatreId == theatreId &&
            s.ScreenName == screenName);

        if (excludeScreenId.HasValue)
        {
            query = query.Where(s =>
                s.ScreenId != excludeScreenId.Value);
        }

        return await query.AnyAsync();
    }

    // Check whether the screen has dependent records.
    public async Task<bool> HasSeatsOrShowsAsync(int screenId)
    {
        bool hasSeats = await _context.Seats
            .AnyAsync(s => s.ScreenId == screenId);

        if (hasSeats)
        {
            return true;
        }

        return await _context.Shows
            .AnyAsync(s => s.ScreenId == screenId);
    }

    // Delete screen.
    public async Task DeleteAsync(Screen screen)
    {
        _context.Screens.Remove(screen);

        await _context.SaveChangesAsync();
    }
}