using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RaceDay.API.Models;

public class Enrolment
{
    [Key]
    public int EnrolmentId { get; set; }

    [ForeignKey(nameof(ParticipantId))]
    public int ParticipantId { get; set; }
    public User Participant { get; set; } = null!;

    [ForeignKey(nameof(EventId))]
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    [ForeignKey(nameof(CategoryId))]
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;
    [Required, MaxLength(20)] public string Status { get; set; } = "Pending";
}
