using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using System.Security.Claims;

[Authorize(Roles = "Participant")]
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public PaymentsController(RaceDayDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> MakePayment(
        [FromBody] MakePaymentDto dto,
        CancellationToken ct)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var ticket = await _db.Tickets
            .Include(t => t.Enrolment)
            .FirstOrDefaultAsync(t => t.Id == dto.TicketId, ct);

        if (ticket is null) return NotFound(new { message = "Ticket not found." });
        if (ticket.ParticipantId != userId) return Forbid();
        if (ticket.PaymentStatus == TicketPaymentStatus.Paid)
            return BadRequest(new { message = "Ticket already paid." });

        if (dto.Amount <= 0 || dto.Amount != ticket.Price)
            return BadRequest(new { message = $"Amount must be exactly {ticket.Price:C}." });

        var reference = $"RD-{ticket.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        var payment = new Payment
        {
            TicketId = ticket.Id,
            Amount = dto.Amount,
            Reference = reference,
            Status = TicketPaymentStatus.Paid,
            PaidAt = DateTime.UtcNow
        };

        ticket.PaymentStatus = TicketPaymentStatus.Paid;
        ticket.PurchaseDate = DateTime.UtcNow;

        if (ticket.Enrolment != null)
            ticket.Enrolment.Status = "Confirmed";

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync(ct);

        return Ok(new
        {
            payment.Id,
            payment.Reference,
            payment.Amount,
            payment.Status,
            Ticket = new
            {
                ticket.Id,
                ticket.TicketNumber,
                ticket.TicketType,
                ticket.Price,
                ticket.PaymentStatus,
                ticket.PurchaseDate
            },
            message = "Payment successful. Ticket is now confirmed."
        });
    }

    // GET api/payments/{reference}
    [HttpGet("{reference}")]
    public async Task<IActionResult> Get(string reference, CancellationToken ct)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var payment = await _db.Payments
            .Include(p => p.Ticket)
            .FirstOrDefaultAsync(p => p.Reference == reference, ct);

        if (payment is null) return NotFound();
        if (payment.Ticket.ParticipantId != userId)
            return Forbid();

        return Ok(payment);
    }
}