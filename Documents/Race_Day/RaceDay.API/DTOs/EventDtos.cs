using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.DTOs;

public class EventCreateDto
{
    [Required, MaxLength(200)] public string Title { get; set; } = null!;
    [Required] public string Description { get; set; } = null!;
    [Required] public DateTime Date { get; set; }
    [Required, MaxLength(200)] public string Location { get; set; } = null!;
    public double Distance { get; set; }
    [Required, RegularExpression("Run|Walk|Cycle")] public string EventType { get; set; } = "Run";
    public IFormFile? BannerImage { get; set; }
    public List<string>? Categories { get; set; }
}
public class EventUpdateDto
{
    [Required, MaxLength(200)] public string Title { get; set; } = null!;
    [Required] public string Description { get; set; } = null!;
    [Required] public DateTime Date { get; set; }
    [Required, MaxLength(200)] public string Location { get; set; } = null!;
    public double Distance { get; set; }
    [Required, RegularExpression("Run|Walk|Cycle")] public string EventType { get; set; } = "Run";
}
public class EventListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Location { get; set; } = null!;
    public double Distance { get; set; }
    public string EventType { get; set; } = null!;
    public string? BannerImageUrl { get; set; }
    public string OrganiserName { get; set; } = null!;
    public int OrganiserId { get; set; }
}
