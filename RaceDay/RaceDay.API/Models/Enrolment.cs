using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.Models;

public class Enrolment
{
    public int Id { get; set; }
    public int ParticipantId { get; set; }
    public User Participant { get; set; } = null!;
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;
    [Required, MaxLength(20)] public string Status { get; set; } = "Pending";
}
