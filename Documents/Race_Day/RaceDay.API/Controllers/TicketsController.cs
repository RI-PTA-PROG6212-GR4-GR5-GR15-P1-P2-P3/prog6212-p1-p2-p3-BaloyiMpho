using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public TicketsController(RaceDayDbContext db) => _db = db;


    [HttpGet("me")]
    [Authorize(Roles = "Participant")]

    // GET api/tickets/mine  → participant's tickets
    [HttpGet("mine")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();
        var tickets = await _db.Tickets
            .Include(t => t.Event)
            .Where(t => t.ParticipantId == userId)
            .ToListAsync(ct);
        return Ok(tickets);
    }


    // GET api/tickets/{id}  → participant's own, or organiser/admin
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var ticket = await _db.Tickets
            .Include(t => t.Event)
            .Include(t => t.Payments)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (ticket is null) return NotFound();

        if (!await CanViewTicket(ticket)) return Forbid();

        return Ok(ticket);
    }

    // GET api/tickets/by-number/{ticketNumber}  → convenient lookup
    [HttpGet("by-number/{ticketNumber}")]
    public async Task<IActionResult> GetByNumber(string ticketNumber, CancellationToken ct)
    {
        var ticket = await _db.Tickets
            .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, ct);
        if (ticket is null) return NotFound();
        if (!await CanViewTicket(ticket)) return Forbid();
        return Ok(ticket);
    }

    private async Task<bool> CanViewTicket(Ticket ticket)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return false;

        if (User.IsInRole("Admin")) return true;
        if (ticket.ParticipantId == userId) return true;
        if (User.IsInRole("Organiser"))
        {
            return await _db.Events
                .AnyAsync(e => e.EventId == ticket.EventId && e.OrganiserId == userId);
        }
        return false;
    }
}