namespace RaceDay.MVC.Models;

public class LoginViewModel
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class RegisterViewModel
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "Participant";
}

public class EventViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Date { get; set; }
    public string Location { get; set; } = "";
    public double Distance { get; set; }
    public string EventType { get; set; } = "";
    public string? BannerImageUrl { get; set; }
    public string OrganiserName { get; set; } = "";
    public int OrganiserId { get; set; }
    public List<CategoryViewModel> Categories { get; set; } = new();

    // Added to support the Ticket UI directly from the Event model
    public string CategoryName { get; set; } = "";
    public bool IsPaid { get; set; }
}

public class CategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class EnrolmentViewModel
{
    public int Id { get; set; }
    public int ParticipantId { get; set; }
    public string ParticipantName { get; set; } = "";
    public int EventId { get; set; }
    public string EventTitle { get; set; } = "";
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public DateTime EnrolmentDate { get; set; }
    public string Status { get; set; } = "";
}

public class ResultViewModel
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string EventTitle { get; set; } = "";
    public int ParticipantId { get; set; }
    public string ParticipantName { get; set; } = "";
    public TimeSpan FinishTime { get; set; }
    public int FinishingPosition { get; set; }
}

public class CreateEnrolmentRequest
{
    public int EventId { get; set; }
    public int CategoryId { get; set; }
    public string TicketType { get; set; } = "Standard";
}
public class CreateEventViewModel
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.Today.AddDays(30);
    public string Location { get; set; } = "";
    public double Distance { get; set; } = 10;
    public string EventType { get; set; } = "Run";
    public IFormFile? BannerImage { get; set; }
    public string CategoriesCsv { get; set; } = "Under 20, Senior, 10km";
}

public class EnrolmentResult
{
    public int EnrolmentId { get; set; }
    public string Status { get; set; } = "";
    public TicketViewModel? Ticket { get; set; }
    public string Message { get; set; } = "";
}

public class TicketConfirmationViewModel
{
    public int EnrolmentId { get; set; }
    public int TicketId { get; set; }
    public string TicketNumber { get; set; } = "";
    public string EventName { get; set; } = "";
    public string CategoryName { get; set; } = "";
    public string Location { get; set; } = "";
    public string TicketType { get; set; } = "";
    public decimal Price { get; set; }
    public string PaymentStatus { get; set; } = "";
    public string Status { get; set; } = "";
    public string Message { get; set; } = "";
}

public class PaymentFormViewModel
{
    public int TicketId { get; set; }
    public string TicketNumber { get; set; } = "";
    public string EventName { get; set; } = "";
    public string CategoryName { get; set; } = "";
    public string TicketType { get; set; } = "";
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } = "";
}

// UPDATED: Added details needed for the Ticket UI
public class PaymentResult
{
    public int PaymentId { get; set; }
    public string Status { get; set; } = "";
    public decimal Amount { get; set; }
    public string Message { get; set; } = "";
}
public class TicketViewModel
{
    public int Id { get; set; }
    public int TicketId { get; set; }

    public string EventName { get; set; } = "";
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = "";
    public string CategoryName { get; set; } = "";

    public string TicketNumber { get; set; } = "";
    public string TicketType { get; set; } = "";
    public decimal Price { get; set; }
    public string PaymentStatus { get; set; } = "";

    // Helper property for the UI to easily check if it's paid
    public bool IsPaid => PaymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase);
}

public class TicketMineViewModel
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = "";
    public string TicketType { get; set; } = "";
    public decimal Price { get; set; }
    public string PaymentStatus { get; set; } = "";
    public DateTime PurchaseDate { get; set; }
    public string EventTitle { get; set; } = "";
    public decimal AmountPaid { get; set; }
    public string CategoryName { get; set; } = "";
}