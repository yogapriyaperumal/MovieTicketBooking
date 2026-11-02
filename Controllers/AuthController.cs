using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Auth;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // CUSTOMER REGISTRATION
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        if (!result.Success)
        {
            return BadRequest(new
            {
                success = false,
                errors = result.Errors
            });
        }

        return Ok(new
        {
            success = true,
            data = result.Response
        });
    }

    // USER LOGIN
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.Success)
        {
            return Unauthorized(new
            {
                success = false,
                errors = result.Errors
            });
        }

        return Ok(new
        {
            success = true,
            data = result.Response
        });
    }
}