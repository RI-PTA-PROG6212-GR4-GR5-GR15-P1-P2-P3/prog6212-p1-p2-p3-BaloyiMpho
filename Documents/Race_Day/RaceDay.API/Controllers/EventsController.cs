using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    private readonly IBlobStorageService _blob;

    public EventsController(RaceDayDbContext db, IBlobStorageService blob)
    {
        _db = db;
        _blob = blob;
    }

    /// <summary>Get all events, optionally filtered by eventType (Run, Walk, Cycle).</summary>

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? eventType)
    {
        var query = _db.Events.Include(e => e.Organiser).AsQueryable();

        if (!string.IsNullOrEmpty(eventType))
            query = query.Where(e => e.EventType == eventType);

        var events = await query
            .OrderBy(e => e.Date)
            .Select(e => new EventListDto
            {
                Id = e.EventId,
                Title = e.Title,
                Description = e.Description,
                Date = e.Date,
                Location = e.Location,
                Distance = e.Distance,
                EventType = e.EventType,
                BannerImageUrl = e.BannerImageUrl,
                OrganiserName = e.Organiser.Name,
                OrganiserId = e.OrganiserId
            })
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("getbyType")]
    public Task<IActionResult> GetByType([FromQuery] string? eventType)
        => GetAll(eventType);

    /// <summary>Get a single event with its categories.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var ev = await _db.Events
            .Include(e => e.Organiser)
            .Include(e => e.Categories)
            .FirstOrDefaultAsync(e => e.EventId == id);
        if (ev == null) return NotFound(new { message = "Event not found." });

        return Ok(new
        {
            Id = ev.EventId,
            Title = ev.Title,
            Description = ev.Description,
            Date = ev.Date,
            Location = ev.Location,
            Distance = ev.Distance,
            EventType = ev.EventType,
            BannerImageUrl = ev.BannerImageUrl,
            OrganiserId = ev.OrganiserId,
            OrganiserName = ev.Organiser.Name,
            Categories = ev.Categories
                .OrderBy(c => c.Name)
                .Select(c => new { Id = c.CategoryId, Name = c.Name })
                .ToList()
        });
    }

    /// <summary>Create a new event (Organiser only). Accepts multipart/form-data with optional banner image.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] EventCreateDto dto)
    {
        string? bannerUrl = null;
        if (dto.BannerImage != null && dto.BannerImage.Length > 0)
        {
            using var stream = dto.BannerImage.OpenReadStream();
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.BannerImage.FileName)}";
            bannerUrl = await _blob.UploadAsync(stream, fileName, dto.BannerImage.ContentType);
        }

        var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ev = new Event
        {
            Title = dto.Title, Description = dto.Description, Date = dto.Date,
            Location = dto.Location, Distance = dto.Distance, EventType = dto.EventType,
            BannerImageUrl = bannerUrl, OrganiserId = organiserId
        };
        _db.Events.Add(ev);
        await _db.SaveChangesAsync();

        if (dto.Categories != null)
        {
            foreach (var catName in dto.Categories.Where(c => !string.IsNullOrWhiteSpace(c)))
                _db.Categories.Add(new Category { Name = catName, EventId = ev.EventId });
            await _db.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(Get), new { id = ev.EventId }, new { ev.EventId, ev.Title });
    }

    /// <summary>Update an event (Organiser only, must own event).</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] EventUpdateDto dto)
    {
        var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ev = await _db.Events.FirstOrDefaultAsync(e => e.EventId == id);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != organiserId) return Forbid();

        ev.Title = dto.Title; ev.Description = dto.Description; ev.Date = dto.Date;
        ev.Location = dto.Location; ev.Distance = dto.Distance; ev.EventType = dto.EventType;
        await _db.SaveChangesAsync();
        return Ok(new { message = "Event updated." });
    }

    /// <summary>Delete an event (Organiser only, must own event).</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ev = await _db.Events.FirstOrDefaultAsync(e => e.EventId == id);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != organiserId) return Forbid();

        if (!string.IsNullOrEmpty(ev.BannerImageUrl))
            await _blob.DeleteAsync(ev.BannerImageUrl);

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Event deleted." });
    }

    /// <summary>Get events owned by the current organiser.</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("my-events")]
    public async Task<IActionResult> MyEvents()
    {
        var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var events = await _db.Events.Where(e => e.OrganiserId == organiserId)
            .OrderBy(e => e.Date).ToListAsync();
        return Ok(events);
    }
}
