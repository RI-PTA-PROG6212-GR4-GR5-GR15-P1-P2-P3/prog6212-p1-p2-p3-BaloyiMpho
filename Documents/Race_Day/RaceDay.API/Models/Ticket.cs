using RaceDay.API.Models;

public class Ticket
{
    public int Id { get; set; }

    // FK to the enrolment that produced this ticket
    public int EnrolmentId { get; set; }
    public Enrolment Enrolment { get; set; } = null!;

    // FK to the participant (User)
    public int ParticipantId { get; set; }
    public User Participant { get; set; } = null!;

   
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public decimal Price { get; set; }
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

   
    public string TicketNumber { get; set; } = null!;

    public string PaymentStatus { get; set; } = TicketPaymentStatus.Pending;
    public string TicketType { get; set; } = TicketTypes.Standard;

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}