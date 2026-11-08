using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EnrolmentsController(RaceDayDbContext db) => _db = db;

    /// <summary>Enrol in an event category (Participant only).</summary>
    [Authorize(Roles = "Participant")]
    [HttpPost]
    public async Task<IActionResult> Enrol([FromBody] EnrolmentCreateDto dto)
    {
        var participantId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var eventExists = await _db.Events.AnyAsync(e => e.EventId == dto.EventId);
        if (!eventExists) return BadRequest(new { message = "Event not found." });

        var categoryExists = await _db.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId && c.EventId == dto.EventId);
        if (!categoryExists) return BadRequest(new { message = "Category does not belong to this event." });

        var exists = await _db.Enrolments.AnyAsync(e =>
            e.ParticipantId == participantId && e.EventId == dto.EventId && e.CategoryId == dto.CategoryId);
        if (exists) return BadRequest(new { message = "Already enrolled in this category." });

        var enrolment = new Enrolment
        {
            ParticipantId = participantId,
            EventId = dto.EventId,
            CategoryId = dto.CategoryId,
            Status = "Pending"
        };
       
        _db.Enrolments.Add(enrolment);
       
        await _db.SaveChangesAsync();
        return Ok(new { enrolment.EnrolmentId, enrolment.Status, message = "Enrolment submitted." });
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
        var enrolment = await _db.Enrolments.Include(e => e.Event).FirstOrDefaultAsync(e => e.EventId == id);
        if (enrolment == null) return NotFound();
        if (enrolment.Event.OrganiserId != organiserId) return Forbid();

        enrolment.Status = "Confirmed";
        await _db.SaveChangesAsync();
        return Ok(new { message = "Enrolment confirmed." });
    }
}
