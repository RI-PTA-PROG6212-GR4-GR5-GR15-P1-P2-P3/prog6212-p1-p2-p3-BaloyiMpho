using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.Models;

public class Event
{
    [Key]
    public int EventId { get; set; }
    [Required, MaxLength(200)] public string Title { get; set; } = null!;
    [Required] public string Description { get; set; } = null!;
    [Required] public DateTime Date { get; set; }
    [Required, MaxLength(200)] public string Location { get; set; } = null!;
    public double Distance { get; set; }
    [Required, MaxLength(20)] public string EventType { get; set; } = "Run";
    [MaxLength(500)] public string? BannerImageUrl { get; set; }
    public int OrganiserId { get; set; }
    public User Organiser { get; set; } = null!;
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    public ICollection<Result> Results { get; set; } = new List<Result>();
}
