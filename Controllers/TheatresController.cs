using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTO.Theatres;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TheatresController : ControllerBase
{
    private readonly ITheatreService _theatreService;

    public TheatresController(ITheatreService theatreService)
    {
        _theatreService = theatreService;
    }

    // GET: api/theatres
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var theatres = await _theatreService.GetAllAsync();

        return Ok(theatres);
    }

    // GET: api/theatres/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid theatre ID."
            });
        }

        var theatre = await _theatreService.GetByIdAsync(id);

        if (theatre == null)
        {
            return NotFound(new
            {
                message = "Theatre not found."
            });
        }

        return Ok(theatre);
    }

    // POST: api/theatres
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTheatreRequest request)
    {
        var theatre = await _theatreService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = theatre.TheatreId },
            theatre);
    }

    // PUT: api/theatres/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateTheatreRequest request)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid theatre ID."
            });
        }

        var theatre = await _theatreService.UpdateAsync(id, request);

        if (theatre == null)
        {
            return NotFound(new
            {
                message = "Theatre not found."
            });
        }

        return Ok(theatre);
    }

    // DELETE: api/theatres/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid theatre ID."
            });
        }

        var result = await _theatreService.DeleteAsync(id);

        switch (result)
        {
            case TheatreDeleteResult.Deleted:
                return NoContent();

            case TheatreDeleteResult.NotFound:
                return NotFound(new
                {
                    message = "Theatre not found."
                });

            case TheatreDeleteResult.HasScreens:
                return Conflict(new
                {
                    message = "Cannot delete this theatre because screens are associated with it."
                });

            default:
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred."
                });
        }
    }
}