namespace RaceDay.API.Models;

public class Result
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;
    public int ParticipantId { get; set; }
    public User Participant { get; set; } = null!;
    public TimeSpan FinishTime { get; set; }
    public int FinishingPosition { get; set; }
}
