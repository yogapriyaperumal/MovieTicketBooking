using Microsoft.AspNetCore.Identity;
using MovieTicketBooking.DTOs.Auth;
using MovieTicketBooking.Models;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Success, RegisterResponse? Response, string[] Errors)>
        RegisterAsync(RegisterRequest request)
    {
        // Check whether the email already exists.
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return (false, null, new[] { "Email is already registered." });
        }

        // Check whether the username already exists.
        var existingUsername =
            await _userManager.FindByNameAsync(request.UserName);

        if (existingUsername != null)
        {
            return (false, null, new[] { "Username is already taken." });
        }

        // Create the new user.
        var user = new ApplicationUser
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber
        };

        // Identity securely hashes the password.
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return (
                false,
                null,
                result.Errors.Select(error => error.Description).ToArray()
            );
        }

        // Assign the Customer role.
        var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

        if (!roleResult.Succeeded)
        {
            // Remove the user if role assignment fails.
            await _userManager.DeleteAsync(user);

            return (
                false,
                null,
                roleResult.Errors.Select(error => error.Description).ToArray()
            );
        }

        var response = new RegisterResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = "Customer",
            Message = "Registration successful."
        };

        return (true, response, Array.Empty<string>());
    }
}