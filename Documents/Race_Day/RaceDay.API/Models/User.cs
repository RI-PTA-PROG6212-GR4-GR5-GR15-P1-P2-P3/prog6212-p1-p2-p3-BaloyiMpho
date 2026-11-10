using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required, MaxLength(100)] public string Name { get; set; } = null!;
    [Required, EmailAddress, MaxLength(200)] public string Email { get; set; } = null!;
    [Required] public string PasswordHash { get; set; } = null!;
    [Required] public string? Role { get; set; }
    [MaxLength(500)] public string? ProfilePictureUrl { get; set; }
    public ICollection<Event> OrganisedEvents { get; set; } = new List<Event>();
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    public ICollection<Result> Results { get; set; } = new List<Result>();
}
