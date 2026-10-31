using MovieTicketBooking.DTO.Movies;
using MovieTicketBooking.Models;
using MovieTicketBooking.Respositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<PagedResponse<MovieResponse>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? title)
    {
        // Validate pagination values.
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var movies = await _movieRepository.GetAllAsync(
            pageNumber,
            pageSize,
            title);

        var totalRecords = await _movieRepository.GetCountAsync(title);

        var movieResponses = movies.Select(MapToResponse);

        return new PagedResponse<MovieResponse>
        {
            Items = movieResponses,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(
                (double)totalRecords / pageSize)
        };
    }

    public async Task<MovieResponse?> GetByIdAsync(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);

        if (movie == null)
        {
            return null;
        }

        return MapToResponse(movie);
    }

    public async Task<MovieResponse> CreateAsync(
        CreateMovieRequest request)
    {
        var movie = new Movie
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate.Date,
            Language = request.Language.Trim(),
            Genre = request.Genre.Trim()
        };

        var createdMovie = await _movieRepository.CreateAsync(movie);

        return MapToResponse(createdMovie);
    }

    public async Task<MovieResponse?> UpdateAsync(
        int id,
        UpdateMovieRequest request)
    {
        var movie = await _movieRepository.GetByIdAsync(id);

        if (movie == null)
        {
            return null;
        }

        movie.Title = request.Title.Trim();
        movie.Description = request.Description?.Trim();
        movie.DurationMinutes = request.DurationMinutes;
        movie.ReleaseDate = request.ReleaseDate.Date;
        movie.Language = request.Language.Trim();
        movie.Genre = request.Genre.Trim();

        await _movieRepository.UpdateAsync(movie);

        return MapToResponse(movie);
    }

    public async Task<MovieDeleteResult> DeleteAsync(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);

        if (movie == null)
        {
            return MovieDeleteResult.NotFound;
        }

        // Do not delete a movie that already has shows.
        bool hasShows = await _movieRepository.HasShowsAsync(id);

        if (hasShows)
        {
            return MovieDeleteResult.HasShows;
        }

        await _movieRepository.DeleteAsync(movie);

        return MovieDeleteResult.Deleted;
    }

    private static MovieResponse MapToResponse(Movie movie)
    {
        return new MovieResponse
        {
            MovieId = movie.MovieId,
            Title = movie.Title,
            Description = movie.Description,
            DurationMinutes = movie.DurationMinutes,
            ReleaseDate = movie.ReleaseDate,
            Language = movie.Language,
            Genre = movie.Genre
        };
    }
}