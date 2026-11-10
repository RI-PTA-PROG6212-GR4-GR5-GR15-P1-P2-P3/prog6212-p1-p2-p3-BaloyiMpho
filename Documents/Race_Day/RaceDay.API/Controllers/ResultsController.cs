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
public class ResultsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public ResultsController(RaceDayDbContext db) => _db = db;

    /// <summary>Record a result (Organiser only, must own event).</summary>
    [Authorize(Roles = "Organiser")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ResultCreateDto dto)
    {
        var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isOwner = await _db.Events.AnyAsync(e => e.EventId == dto.EventId && e.OrganiserId == organiserId);
        if (!isOwner) return Forbid();

        var enrolmentExists = await _db.Enrolments.AnyAsync(e =>
            e.EventId == dto.EventId && e.ParticipantId == dto.ParticipantId);
        if (!enrolmentExists) return BadRequest(new { message = "Participant is not enrolled in this event." });

        var existing = await _db.Results.FirstOrDefaultAsync(r =>
            r.EventId == dto.EventId && r.ParticipantId == dto.ParticipantId);
        if (existing != null)
        {
            existing.FinishTime = dto.FinishTime;
            existing.FinishingPosition = dto.FinishingPosition;
        }
        else
        {
            _db.Results.Add(new Result
            {
                EventId = dto.EventId, ParticipantId = dto.ParticipantId,
                FinishTime = dto.FinishTime, FinishingPosition = dto.FinishingPosition
            });
        }
        await _db.SaveChangesAsync();
        return Ok(new { message = "Result recorded." });
    }

    /// <summary>Get current participant's results.</summary>
    [Authorize(Roles = "Participant")]
    [HttpGet("my-results")]
    public async Task<IActionResult> MyResults()
    {
        var participantId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var results = await _db.Results
            .Include(r => r.Event)
            .Where(r => r.ParticipantId == participantId)
            .Select(r => new ResultDto
            {
                Id = r.ResultId, EventId = r.EventId, EventTitle = r.Event.Title,
                ParticipantId = r.ParticipantId, FinishTime = r.FinishTime,
                FinishingPosition = r.FinishingPosition
            }).ToListAsync();
        return Ok(results);
    }

    /// <summary>Get results for an event (Organiser only, must own event).</summary>
    [Authorize(Roles = "Organiser")]
    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> EventResults(int eventId)
    {
        var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isOwner = await _db.Events.AnyAsync(e => e.EventId == eventId && e.OrganiserId == organiserId);
        if (!isOwner) return Forbid();

        var results = await _db.Results
            .Include(r => r.Event)
            .Include(r => r.Participant)
            .Where(r => r.EventId == eventId)
            .OrderBy(r => r.FinishingPosition)
            .Select(r => new ResultDto
            {
                Id = r.ResultId, EventId = r.EventId, EventTitle = r.Event.Title,
                ParticipantId = r.ParticipantId, ParticipantName = r.Participant.Name,
                FinishTime = r.FinishTime, FinishingPosition = r.FinishingPosition
            }).ToListAsync();
        return Ok(results);
    }
}
