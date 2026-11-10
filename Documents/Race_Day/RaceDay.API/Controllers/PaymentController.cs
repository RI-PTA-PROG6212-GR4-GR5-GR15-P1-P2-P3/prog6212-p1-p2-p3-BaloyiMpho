using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using System;
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
            .FirstOrDefaultAsync(t => t.Id == dto.TicketId, ct);

        if (ticket is null) return NotFound(new { message = "Ticket not found." });
        if (ticket.ParticipantId != userId) return Forbid();
        if (ticket.PaymentStatus == TicketPaymentStatus.Paid)
            return BadRequest(new { message = "Ticket already paid." });

        // Server-side amount check
        if (dto.Amount != ticket.Price)
            return BadRequest(new { message = $"Amount must be exactly {ticket.Price:C}." });

        
        var payment = new Payment
        {
            TicketId = ticket.Id,
            Amount = dto.Amount,
          
            PaidAt = DateTime.UtcNow
        };

        ticket.PaymentStatus = TicketPaymentStatus.Paid;

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync(ct);

        return Ok(new
        {
           
            payment.Amount,
            Ticket = new
            {
                ticket.TicketNumber,
                ticket.PaymentStatus
            }
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