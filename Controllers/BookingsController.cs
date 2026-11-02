using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _service;

    public BookingsController(IBookingService service)
    {
        _service = service;
    }

    // GET: api/bookings
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _service.GetAllAsync();

        return Ok(bookings);
    }

    // GET: api/bookings/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _service.GetByIdAsync(id);

        if (booking == null)
        {
            return NotFound(new { message = "Booking not found." });
        }

        return Ok(booking);
    }

    // GET: api/bookings/reference/MTB-...
    [HttpGet("reference/{reference}")]
    public async Task<IActionResult> GetByReference(string reference)
    {
        var booking = await _service.GetByReferenceAsync(reference);

        if (booking == null)
        {
            return NotFound(new { message = "Booking not found." });
        }

        return Ok(booking);
    }

    // POST: api/bookings
    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingRequest request)
    {
        var (booking, error) = await _service.CreateAsync(request);

        if (error != null)
        {
            if (error == "Show not found.")
            {
                return NotFound(new { message = error });
            }

            return Conflict(new { message = error });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = booking!.BookingId },
            booking);
    }

    // PUT: api/bookings/1/cancel
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var (response, error) = await _service.CancelAsync(id);

        if (error != null)
        {
            if (error == "Booking not found.")
            {
                return NotFound(new { message = error });
            }

            return Conflict(new { message = error });
        }

        return Ok(response);
    }
}