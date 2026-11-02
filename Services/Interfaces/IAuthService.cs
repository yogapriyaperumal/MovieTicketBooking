using MovieTicketBooking.DTOs.Auth;

namespace MovieTicketBooking.Services.Interfaces;

public interface IAuthService
{
    Task<(bool Success, RegisterResponse? Response, string[] Errors)>
        RegisterAsync(RegisterRequest request);
}