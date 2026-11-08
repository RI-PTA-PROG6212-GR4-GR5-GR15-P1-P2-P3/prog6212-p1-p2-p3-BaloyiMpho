using RaceDay.API.Models;
using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.DTOs;

public class RegisterDto
{
   // [Key] public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = null!;
    [Required, EmailAddress] public string Email { get; set; } = null!;
    [Required, MinLength(6)] public string Password { get; set; } = null!;
    [Required, RegularExpression("Organiser|Participant")] public string Role { get; set; } = "Participant";
}
public class LoginDto
{
   // [Key] public int Id { get; set; }
    [Required, EmailAddress] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
}
public class LoginResponseDto
{

    public string Token { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Role { get; set; } = null!;
}
