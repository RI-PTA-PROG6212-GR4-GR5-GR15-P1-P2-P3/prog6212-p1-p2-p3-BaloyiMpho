using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class ParticipantDto
    {
       
            public int ParticipantId { get; set; }

            [Required]
            public string ParticipantName { get; set; } = string.Empty;

            public string? ParticipantType { get; set; }
        
    }
}

