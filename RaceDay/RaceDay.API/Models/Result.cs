using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models;

public class Result
{
    [Key]
    public int ResultId { get; set; }

    [Required]
    public TimeSpan FinishTime { get; set; }

    public int FinishingPosition { get; set; }

    // Foreign keys
    public int EventId { get; set; }
    public int ParticipantId { get; set; }

    // Navigation properties
    public Event? Event { get; set; }
    public Participant? Participant { get; set; }
}