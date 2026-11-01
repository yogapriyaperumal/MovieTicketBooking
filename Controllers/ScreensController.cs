using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTO.Screens;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScreensController : ControllerBase
{
    private readonly IScreenService _screenService;

    public ScreensController(IScreenService screenService)
    {
        _screenService = screenService;
    }

    // GET: api/screens
    // GET: api/screens?theatreId=1
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? theatreId)
    {
        if (theatreId.HasValue && theatreId.Value <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid theatre ID."
            });
        }

        var screens = await _screenService.GetAllAsync(theatreId);

        return Ok(screens);
    }

    // GET: api/screens/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid screen ID."
            });
        }

        var screen = await _screenService.GetByIdAsync(id);

        if (screen == null)
        {
            return NotFound(new
            {
                message = "Screen not found."
            });
        }

        return Ok(screen);
    }

    // GET: api/screens/theatre/1
    [HttpGet("theatre/{theatreId:int}")]
    public async Task<IActionResult> GetByTheatre(int theatreId)
    {
        if (theatreId <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid theatre ID."
            });
        }

        var screens = await _screenService.GetAllAsync(theatreId);

        return Ok(screens);
    }

    // POST: api/screens
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateScreenRequest request)
    {
        var result = await _screenService.CreateAsync(request);

        switch (result)
        {
            case ScreenCreateResult.Created:
                return StatusCode(201, new
                {
                    message = "Screen created successfully."
                });

            case ScreenCreateResult.TheatreNotFound:
                return NotFound(new
                {
                    message = "The specified theatre does not exist."
                });

            case ScreenCreateResult.DuplicateName:
                return Conflict(new
                {
                    message = "A screen with this name already exists in this theatre."
                });

            default:
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred."
                });
        }
    }

    // PUT: api/screens/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateScreenRequest request)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid screen ID."
            });
        }

        var result = await _screenService.UpdateAsync(id, request);

        switch (result)
        {
            case ScreenUpdateResult.Updated:
                return Ok(new
                {
                    message = "Screen updated successfully."
                });

            case ScreenUpdateResult.ScreenNotFound:
                return NotFound(new
                {
                    message = "Screen not found."
                });

            case ScreenUpdateResult.TheatreNotFound:
                return NotFound(new
                {
                    message = "The specified theatre does not exist."
                });

            case ScreenUpdateResult.DuplicateName:
                return Conflict(new
                {
                    message = "A screen with this name already exists in this theatre."
                });

            default:
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred."
                });
        }
    }

    // DELETE: api/screens/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid screen ID."
            });
        }

        var result = await _screenService.DeleteAsync(id);

        switch (result)
        {
            case ScreenDeleteResult.Deleted:
                return NoContent();

            case ScreenDeleteResult.ScreenNotFound:
                return NotFound(new
                {
                    message = "Screen not found."
                });

            case ScreenDeleteResult.HasDependencies:
                return Conflict(new
                {
                    message = "Cannot delete this screen because seats or shows are associated with it."
                });

            default:
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred."
                });
        }
    }
}