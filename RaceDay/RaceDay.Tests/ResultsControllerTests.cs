using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests;

public class ResultsControllerTests
{
    private static RaceDayDbContext GetDb()
    {
        var opts = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new RaceDayDbContext(opts);
    }

    private static ResultsController WithUser(ResultsController c, int id, string role)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()), new Claim(ClaimTypes.Role, role) };
        c.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test")) }
        };
        return c;
    }

    [Fact]
    public async Task Create_ValidOrganiser_ReturnsOk()
    {
        var db = GetDb();
        db.Users.Add(new User { UserId = 1, Name = "O", Email = "o@t.com", PasswordHash = "h", Role = "Organiser" });
        db.Users.Add(new User { UserId = 2, Name = "P", Email = "p@t.com", PasswordHash = "h", Role = "Participant" });
        db.Events.Add(new Event { EventId = 1, Title = "E", Description = "D", Date = DateTime.UtcNow, Location = "L", Distance = 10, EventType = "Run", OrganiserId = 1 });
        db.Categories.Add(new Category { CategoryId = 1, Name = "Senior", EventId = 1 });
        db.Enrolments.Add(new Enrolment { ParticipantId = 2, EventId = 1, CategoryId = 1, Status = "Confirmed" });
        await db.SaveChangesAsync();

        var controller = WithUser(new ResultsController(db), 1, "Organiser");
        var result = await controller.Create(new ResultCreateDto
        {
            EventId = 1, ParticipantId = 2,
            FinishTime = TimeSpan.FromMinutes(45), FinishingPosition = 1
        });

        Assert.IsType<OkObjectResult>(result);
        Assert.Single(db.Results);
    }

    [Fact]
    public async Task Create_NotOwner_ReturnsForbid()
    {
        var db = GetDb();
        db.Users.Add(new User { UserId = 1, Name = "O", Email = "o@t.com", PasswordHash = "h", Role = "Organiser" });
        db.Users.Add(new User { UserId = 2, Name = "O2", Email = "o2@t.com", PasswordHash = "h", Role = "Organiser" });
        db.Users.Add(new User { UserId = 3, Name = "P", Email = "p@t.com", PasswordHash = "h", Role = "Participant" });
        db.Events.Add(new Event { EventId = 1, Title = "E", Description = "D", Date = DateTime.UtcNow, Location = "L", Distance = 10, EventType = "Run", OrganiserId = 1 });
        db.Categories.Add(new Category { CategoryId = 1, Name = "Senior", EventId = 1 });
        db.Enrolments.Add(new Enrolment { ParticipantId = 3, EventId = 1, CategoryId = 1, Status = "Confirmed" });
        await db.SaveChangesAsync();

        var controller = WithUser(new ResultsController(db), 2, "Organiser");
        var result = await controller.Create(new ResultCreateDto
        {
            EventId = 1, ParticipantId = 3,
            FinishTime = TimeSpan.FromMinutes(50), FinishingPosition = 2
        });

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Create_ParticipantNotEnrolled_ReturnsBadRequest()
    {
        var db = GetDb();
        db.Users.Add(new User { UserId = 1, Name = "O", Email = "o@t.com", PasswordHash = "h", Role = "Organiser" });
        db.Events.Add(new Event { EventId = 1, Title = "E", Description = "D", Date = DateTime.UtcNow, Location = "L", Distance = 10, EventType = "Run", OrganiserId = 1 });
        await db.SaveChangesAsync();

        var controller = WithUser(new ResultsController(db), 1, "Organiser");
        var result = await controller.Create(new ResultCreateDto
        {
            EventId = 1, ParticipantId = 99,
            FinishTime = TimeSpan.FromMinutes(45), FinishingPosition = 1
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
