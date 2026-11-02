using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MovieTicketBooking.DTOs.Auth;
using MovieTicketBooking.Models;
using MovieTicketBooking.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieTicketBooking.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    // CUSTOMER REGISTRATION
    public async Task<(bool Success, RegisterResponse? Response, string[] Errors)>
        RegisterAsync(RegisterRequest request)
    {
        var existingUser =
            await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return (false, null, new[] { "Email is already registered." });
        }

        var existingUsername =
            await _userManager.FindByNameAsync(request.UserName);

        if (existingUsername != null)
        {
            return (false, null, new[] { "Username is already taken." });
        }

        var user = new ApplicationUser
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber
        };

        var result = await _userManager.CreateAsync(
            user, request.Password);

        if (!result.Succeeded)
        {
            return (
                false,
                null,
                result.Errors.Select(e => e.Description).ToArray()
            );
        }

        var roleResult =
            await _userManager.AddToRoleAsync(user, "Customer");

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return (
                false,
                null,
                roleResult.Errors.Select(e => e.Description).ToArray()
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

    // CUSTOMER LOGIN
    public async Task<(bool Success, AuthResponse? Response, string[] Errors)>
        LoginAsync(LoginRequest request)
    {
        ApplicationUser? user;

        if (request.UserNameOrEmail.Contains('@'))
        {
            user = await _userManager.FindByEmailAsync(
                request.UserNameOrEmail);
        }
        else
        {
            user = await _userManager.FindByNameAsync(
                request.UserNameOrEmail);
        }

        if (user == null)
        {
            return (false, null, new[] { "Invalid username/email or password." });
        }

        var passwordValid = await _userManager.CheckPasswordAsync(
            user, request.Password);

        if (!passwordValid)
        {
            return (false, null, new[] { "Invalid username/email or password." });
        }

        var roles = await _userManager.GetRolesAsync(user);

        var jwtSettings = _configuration.GetSection("Jwt");

        var key = jwtSettings["Key"]
            ?? throw new InvalidOperationException("JWT Key is missing.");

        var issuer = jwtSettings["Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is missing.");

        var audience = jwtSettings["Audience"]
            ?? throw new InvalidOperationException("JWT Audience is missing.");

        var expirationMinutes = int.Parse(
            jwtSettings["ExpirationMinutes"] ?? "60");

        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        var response = new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList(),
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            TokenType = "Bearer"
        };

        return (true, response, Array.Empty<string>());
    }
}