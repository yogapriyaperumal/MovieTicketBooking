using MovieTicketBooking.DTO.Theatres;
using MovieTicketBooking.Models;
using MovieTicketBooking.Respositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class TheatreService : ITheatreService
    {
        private readonly ITheatreRepository _theatreRepository;

        public TheatreService(ITheatreRepository theatreRepository)
        {
            _theatreRepository = theatreRepository;
        }

        public async Task<List<TheatreResponse>> GetAllAsync()
        {
            var theatres = await _theatreRepository.GetAllAsync();

            return theatres.Select(MapToResponse).ToList();
        }

        public async Task<TheatreResponse?> GetByIdAsync(int id)
        {
            var theatre = await _theatreRepository.GetByIdAsync(id);

            if (theatre == null)
            {
                return null;
            }

            return MapToResponse(theatre);
        }

        public async Task<TheatreResponse> CreateAsync(
            CreateTheatreRequest request)
        {
            var theatre = new Theatre
            {
                Name = request.Name.Trim(),
                Location = request.Location.Trim()
            };

            var createdTheatre =
                await _theatreRepository.CreateAsync(theatre);

            return MapToResponse(createdTheatre);
        }

        public async Task<TheatreResponse?> UpdateAsync(
            int id,
            UpdateTheatreRequest request)
        {
            var theatre = await _theatreRepository.GetByIdAsync(id);

            if (theatre == null)
            {
                return null;
            }

            theatre.Name = request.Name.Trim();
            theatre.Location = request.Location.Trim();

            await _theatreRepository.UpdateAsync(theatre);

            return MapToResponse(theatre);
        }

        public async Task<TheatreDeleteResult> DeleteAsync(int id)
        {
            var theatre = await _theatreRepository.GetByIdAsync(id);

            if (theatre == null)
            {
                return TheatreDeleteResult.NotFound;
            }

            // A theatre with screens cannot be deleted.
            bool hasScreens =
                await _theatreRepository.HasScreensAsync(id);

            if (hasScreens)
            {
                return TheatreDeleteResult.HasScreens;
            }

            await _theatreRepository.DeleteAsync(theatre);

            return TheatreDeleteResult.Deleted;
        }

        private static TheatreResponse MapToResponse(Theatre theatre)
        {
            return new TheatreResponse
            {
                TheatreId = theatre.TheatreId,
                Name = theatre.Name,
                Location = theatre.Location
            };
        }
    }
}
