using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTO.Movies;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    // GET: api/movies
    [Authorize(Policy = "AdminOrCustomer")]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? title = null)
    {
        var movies = await _movieService.GetAllAsync(
            pageNumber,
            pageSize,
            title);

        return Ok(movies);
    }

    // GET: api/movies/search?title=leo
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string title,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest(new
            {
                message = "Movie title is required for searching."
            });
        }

        var movies = await _movieService.GetAllAsync(
            pageNumber,
            pageSize,
            title);

        return Ok(movies);
    }

    // GET: api/movies/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid movie ID."
            });
        }

        var movie = await _movieService.GetByIdAsync(id);

        if (movie == null)
        {
            return NotFound(new
            {
                message = "Movie not found."
            });
        }

        return Ok(movie);
    }

    // POST: api/movies
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateMovieRequest request)
    {
        var movie = await _movieService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = movie.MovieId },
            movie);
    }

    // PUT: api/movies/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateMovieRequest request)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid movie ID."
            });
        }

        var movie = await _movieService.UpdateAsync(id, request);

        if (movie == null)
        {
            return NotFound(new
            {
                message = "Movie not found."
            });
        }

        return Ok(movie);
    }

    // DELETE: api/movies/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid movie ID."
            });
        }

        var result = await _movieService.DeleteAsync(id);

        switch (result)
        {
            case MovieDeleteResult.Deleted:
                return NoContent();

            case MovieDeleteResult.NotFound:
                return NotFound(new
                {
                    message = "Movie not found."
                });

            case MovieDeleteResult.HasShows:
                return Conflict(new
                {
                    message = "Cannot delete this movie because shows are already scheduled for it."
                });

            default:
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred."
                });
        }
    }
}