using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowsController : ControllerBase
{
    private readonly IShowService _service;

    public ShowsController(IShowService service)
    {
        _service = service;
    }

    // GET: api/shows
    // GET: api/shows?movieId=1
    // GET: api/shows?screenId=1
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? movieId,
        [FromQuery] int? screenId)
    {
        var shows = await _service.GetAllAsync(movieId, screenId);

        return Ok(shows);
    }

    // GET: api/shows/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var show = await _service.GetByIdAsync(id);

        if (show == null)
        {
            return NotFound(new { message = "Show not found." });
        }

        return Ok(show);
    }

    // GET: api/shows/movie/1
    [HttpGet("movie/{movieId:int}")]
    public async Task<IActionResult> GetByMovie(int movieId)
    {
        var shows = await _service.GetAllAsync(movieId, null);

        return Ok(shows);
    }

    // GET: api/shows/screen/1
    [HttpGet("screen/{screenId:int}")]
    public async Task<IActionResult> GetByScreen(int screenId)
    {
        var shows = await _service.GetAllAsync(null, screenId);

        return Ok(shows);
    }

    // POST: api/shows
    [HttpPost]
    public async Task<IActionResult> Create(CreateShowRequest request)
    {
        var (show, error) = await _service.CreateAsync(request);

        if (error != null)
        {
            if (error == "Movie not found." ||
                error == "Screen not found.")
            {
                return NotFound(new { message = error });
            }

            return Conflict(new { message = error });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = show!.ShowId },
            show);
    }

    // PUT: api/shows/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateShowRequest request)
    {
        var (success, error) = await _service.UpdateAsync(id, request);

        if (!success)
        {
            if (error == "Show not found." ||
                error == "Movie not found." ||
                error == "Screen not found.")
            {
                return NotFound(new { message = error });
            }

            return Conflict(new { message = error });
        }

        return Ok(new { message = "Show updated successfully." });
    }

    // DELETE: api/shows/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _service.DeleteAsync(id);

        if (!success)
        {
            if (error == "Show not found.")
            {
                return NotFound(new { message = error });
            }

            return Conflict(new { message = error });
        }

        return NoContent();
    }
}