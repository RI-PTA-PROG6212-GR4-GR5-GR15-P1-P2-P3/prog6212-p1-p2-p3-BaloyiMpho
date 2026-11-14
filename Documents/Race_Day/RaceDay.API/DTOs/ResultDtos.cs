using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.DTOs;

public class ResultCreateDto
{
    [Required] public int EventId { get; set; }
    [Required] public int ParticipantId { get; set; }
    [Required] public TimeSpan FinishTime { get; set; }
    [Required, Range(1, int.MaxValue)] public int FinishingPosition { get; set; }
}
public class ResultDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string EventTitle { get; set; } = null!;
    public int ParticipantId { get; set; }
    public string ParticipantName { get; set; } = null!;
    public TimeSpan FinishTime { get; set; }
    public int FinishingPosition { get; set; }
}
