using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        
        [Required]
        [MaxLength(50)]
        public string Reference { get; set; } = null!;

        
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        // FK → Ticket
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;

       
        [MaxLength(20)]
        public string Status { get; set; } = "Paid";
    }
}