using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Result
{
    [Key]
    public int ResultId { get; set; }

    [Required]
    public TimeSpan FinishTime { get; set; }

    public int FinishingPosition { get; set; }

    public int EventId { get; set; }
    public Event? Event { get; set; }

    public int ParticipantId { get; set; }
    public User? Participant { get; set; }
}