using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeatsController : ControllerBase
{
    private readonly ISeatService _service;

    public SeatsController(ISeatService service)
    {
        _service = service;
    }

    // GET: api/seats?screenId=1
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? screenId)
    {
        var seats = await _service.GetAllAsync(screenId);

        return Ok(seats);
    }

    // GET: api/seats/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var seat = await _service.GetByIdAsync(id);

        if (seat == null)
        {
            return NotFound(new { message = "Seat not found." });
        }

        return Ok(seat);
    }

    // POST: api/seats
    [HttpPost]
    public async Task<IActionResult> Create(CreateSeatRequest request)
    {
        var (seat, error) = await _service.CreateAsync(request);

        if (error != null)
        {
            if (error == "Screen not found.")
            {
                return NotFound(new { message = error });
            }

            return Conflict(new { message = error });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = seat!.SeatId },
            seat);
    }

    // POST: api/seats/bulk
    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreate(
        BulkCreateSeatsRequest request)
    {
        var (seats, error) = await _service.BulkCreateAsync(request);

        if (error != null)
        {
            if (error == "Screen not found.")
            {
                return NotFound(new { message = error });
            }

            return Conflict(new { message = error });
        }

        return Ok(new
        {
            message = "Seats created successfully.",
            count = seats!.Count,
            seats
        });
    }

    // PUT: api/seats/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateSeatRequest request)
    {
        var (success, error) = await _service.UpdateAsync(id, request);

        if (!success)
        {
            if (error == "Seat not found.")
            {
                return NotFound(new { message = error });
            }

            return Conflict(new { message = error });
        }

        return Ok(new { message = "Seat updated successfully." });
    }

    // DELETE: api/seats/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _service.DeleteAsync(id);

        if (!success)
        {
            if (error == "Seat not found.")
            {
                return NotFound(new { message = error });
            }

            return Conflict(new { message = error });
        }

        return NoContent();
    }
}
