using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models;

public class Participant
{
    [Key]
    public int ParticipantId { get; set; }

    [Required]
    public string ParticipantName { get; set; } = string.Empty;

    public string? ParticipantType { get; set; }

    // Navigation properties
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();

    public ICollection<Result> Results { get; set; } = new List<Result>();
}