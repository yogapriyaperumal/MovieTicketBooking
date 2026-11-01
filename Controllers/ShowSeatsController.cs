using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/show-seats")]
public class ShowSeatsController : ControllerBase
{
    private readonly IShowSeatService _service;

    public ShowSeatsController(IShowSeatService service)
    {
        _service = service;
    }

    // GET: api/show-seats/show/1
    [HttpGet("show/{showId:int}")]
    public async Task<IActionResult> GetByShowId(int showId)
    {
        var (seats, error) = await _service.GetByShowIdAsync(showId);

        if (error != null)
        {
            return NotFound(new { message = error });
        }

        return Ok(seats);
    }

    // GET: api/show-seats/show/1/available
    [HttpGet("show/{showId:int}/available")]
    public async Task<IActionResult> GetAvailable(int showId)
    {
        var (seats, error) =
            await _service.GetAvailableByShowIdAsync(showId);

        if (error != null)
        {
            return NotFound(new { message = error });
        }

        return Ok(seats);
    }

    // GET: api/show-seats/show/1/booked
    [HttpGet("show/{showId:int}/booked")]
    public async Task<IActionResult> GetBooked(int showId)
    {
        var (seats, error) =
            await _service.GetBookedByShowIdAsync(showId);

        if (error != null)
        {
            return NotFound(new { message = error });
        }

        return Ok(seats);
    }

    // GET: api/show-seats/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var seat = await _service.GetByIdAsync(id);

        if (seat == null)
        {
            return NotFound(new { message = "Show seat not found." });
        }

        return Ok(seat);
    }
}