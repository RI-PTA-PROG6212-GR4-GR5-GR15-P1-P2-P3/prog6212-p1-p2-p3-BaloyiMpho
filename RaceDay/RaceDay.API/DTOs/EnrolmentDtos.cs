using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.DTOs;

public class EnrolmentCreateDto
{
    [Required] public int EventId { get; set; }
    [Required] public int CategoryId { get; set; }
}
public class EnrolmentDto
{
    public int Id { get; set; }
    public int ParticipantId { get; set; }
    public string ParticipantName { get; set; } = null!;

    public int EventId { get; set; }
    public string EventTitle { get; set; } = null!;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public DateTime EnrolmentDate { get; set; }
    public string Status { get; set; } = null!;
}
