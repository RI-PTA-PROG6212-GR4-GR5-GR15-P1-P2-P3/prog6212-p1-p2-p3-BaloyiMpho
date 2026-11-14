using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using RaceDay.API.Services;
using System.Net.Sockets;
using System.Security.Claims;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public ITicketNumberGenerator _ticketNumberGenerator;
    public readonly ILogger<EnrolmentsController> _logger;
    public EnrolmentsController(RaceDayDbContext db, ITicketNumberGenerator ticketNumberGenerator, ILogger<EnrolmentsController> logger)
    {
        _db = db;
        _ticketNumberGenerator = ticketNumberGenerator;
        _logger = logger;
    }

    /// <summary>Enrol in an event category (Participant only).</summary>
    [Authorize(Roles = "Participant")]
    [HttpPost]
    public async Task<IActionResult> Enrol(
        [FromBody] EnrolmentCreateDto dto,
        CancellationToken ct)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var participantId))
            return Unauthorized();

        var ticketType = string.IsNullOrWhiteSpace(dto.TicketType)
            ? TicketTypes.Standard
            : dto.TicketType.Trim();

        if (ticketType is not (TicketTypes.Standard or TicketTypes.Vip or TicketTypes.Student))
            return BadRequest(new { message = $"Unknown ticket type '{ticketType}'." });

        var eventExists = await _db.Events.AnyAsync(e => e.EventId == dto.EventId, ct);
        if (!eventExists) return BadRequest(new { message = "Event not found." });

        var categoryExists = await _db.Categories
            .AnyAsync(c => c.CategoryId == dto.CategoryId && c.EventId == dto.EventId, ct);
        if (!categoryExists)
            return BadRequest(new { message = "Category does not belong to this event." });

        var alreadyEnrolled = await _db.Enrolments
            .AnyAsync(e => e.ParticipantId == participantId && e.EventId == dto.EventId, ct);
        if (alreadyEnrolled)
            return BadRequest(new { message = "You are already enrolled in this event." });

        var enrolment = new Enrolment
        {
            ParticipantId = participantId,
            EventId = dto.EventId,
            CategoryId = dto.CategoryId,
            EnrolmentDate = DateTime.UtcNow,
            Status = "Pending"
        };

        var generatedTicketNumber = await _ticketNumberGenerator.GenerateAsync(dto.EventId, ct);
        var ticketPrice = TicketPrices.GetPrice(ticketType);

        var ticket = new Ticket
        {
            Enrolment = enrolment,
            ParticipantId = participantId,
            EventId = dto.EventId,
            TicketType = ticketType,
            Price = ticketPrice,
            PurchaseDate = DateTime.UtcNow,
            TicketNumber = generatedTicketNumber,
            PaymentStatus = TicketPaymentStatus.Pending
        };

        _db.Enrolments.Add(enrolment);
        _db.Tickets.Add(ticket);

        await _db.SaveChangesAsync(ct);

        var category = await _db.Categories
            .Where(c => c.CategoryId == dto.CategoryId)
            .Select(c => c.Name)
            .FirstAsync(ct);

        var eventInfo = await _db.Events
            .Where(e => e.EventId == dto.EventId)
            .Select(e => new { e.Title, e.Date, e.Location })
            .FirstAsync(ct);

        return Ok(new
        {
            enrolment.EnrolmentId,
            enrolment.Status,
            Ticket = new
            {
                ticket.Id,
                ticket.TicketNumber,
                ticket.TicketType,
                Price = ticket.Price,
                ticket.PaymentStatus,
                EventName = eventInfo.Title,
                EventDate = eventInfo.Date,
                Location = eventInfo.Location,
                CategoryName = category
            },
            message = "Enrolment submitted. Awaiting payment."
        });
    }

    /// <summary>Get current participant's enrolments.</summary>
    [Authorize(Roles = "Participant")]
    [HttpGet("my-enrolments")]
    public async Task<IActionResult> MyEnrolments()
    {
        var participantId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var list = await _db.Enrolments
            .Include(e => e.Event)
            .Include(e => e.Category)
            .Where(e => e.ParticipantId == participantId)
            .Select(e => new EnrolmentDto
            {
                Id = e.EnrolmentId, ParticipantId = e.ParticipantId, EventId = e.EventId,
                EventTitle = e.Event.Title, CategoryId = e.CategoryId,
                CategoryName = e.Category.Name, EnrolmentDate = e.EnrolmentDate,
                Status = e.Status
            }).ToListAsync();
        return Ok(list);
    }

    /// <summary>Get enrolments for a specific event (Organiser only, must own event).</summary>
    [Authorize(Roles = "Organiser")]
    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> EventEnrolments(int eventId)
    {
        var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isOwner = await _db.Events.AnyAsync(e => e.EventId == eventId && e.OrganiserId == organiserId);
        if (!isOwner) return Forbid();

        var list = await _db.Enrolments
            .Include(e => e.Participant)
            .Include(e => e.Category)
            .Include(e => e.Event)
            .Where(e => e.EventId == eventId)
            .Select(e => new EnrolmentDto
            {
                Id = e.EnrolmentId, ParticipantId = e.ParticipantId,
                ParticipantName = e.Participant.Name, EventId = e.EventId,
                EventTitle = e.Event.Title, CategoryId = e.CategoryId,
                CategoryName = e.Category.Name, EnrolmentDate = e.EnrolmentDate,
                Status = e.Status
            }).ToListAsync();
        return Ok(list);
    }

    /// <summary>Confirm an enrolment (Organiser only, must own event).</summary>
    [Authorize(Roles = "Organiser")]
    [HttpPut("{id}/confirm")]
    public async Task<IActionResult> Confirm(int id)
    {
        var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var enrolment = await _db.Enrolments.Include(e => e.Event).FirstOrDefaultAsync(e => e.EnrolmentId == id);
        if (enrolment == null) return NotFound();
        if (enrolment.Event.OrganiserId != organiserId) return Forbid();

        enrolment.Status = "Confirmed";
        await _db.SaveChangesAsync();
        return Ok(new { message = "Enrolment confirmed." });
    }
}
