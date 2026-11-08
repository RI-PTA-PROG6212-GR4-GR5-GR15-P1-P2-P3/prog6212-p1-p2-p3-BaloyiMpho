using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    private readonly IConfiguration _config;
    private IPasswordHasher<User> _hasher;
    public AuthController(RaceDayDbContext db, IConfiguration config, IPasswordHasher<User> hasher)
    {
        _db = db;
        _config = config;
        _hasher = hasher;
    }

    /// <summary>Register a new user (Organiser or Participant).</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest(new { message = "Email already exists." });

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = "Participant",
            PasswordHash = null,
        };
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Registered successfully.", userId = user.UserId });
    }

    /// <summary>Login and receive a JWT token.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null )
            return Unauthorized(new { message = "Invalid email or password." });
        var PasswordHash = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if(PasswordHash == PasswordVerificationResult.Failed) 
        {
            return Unauthorized("Invalid email or password");
        }

        var token = GenerateJwtToken(user);
        return Ok(new LoginResponseDto { Token = token, UserName = user.Name, Role = user.Role });
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddHours(double.Parse(_config["Jwt:ExpiryHours"] ?? "3"));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
