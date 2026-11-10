using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class MakePaymentDto
    {
        [Required] public int TicketId { get; set; }
        [Required] public decimal Amount { get; set; }
    }
}
