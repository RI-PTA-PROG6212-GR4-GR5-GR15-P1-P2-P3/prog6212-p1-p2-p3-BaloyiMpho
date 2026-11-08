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

public class EnrolmentsControllerTests
{
    private static RaceDayDbContext GetDb()
    {
        var opts = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new RaceDayDbContext(opts);
    }

    private static EnrolmentsController WithUser(EnrolmentsController c, int id, string role)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()), new Claim(ClaimTypes.Role, role) };
        c.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test")) }
        };
        return c;
    }

    [Fact]
    public async Task Enrol_ValidParticipant_ReturnsOk()
    {
        var db = GetDb();
        db.Users.Add(new User { UserId = 1, Name = "P", Email = "p@t.com", PasswordHash = "h", Role = "Participant" });
        db.Events.Add(new Event { EventId = 1, Title = "E", Description = "D", Date = DateTime.UtcNow, Location = "L", Distance = 10, EventType = "Run", OrganiserId = 2 });
        db.Categories.Add(new Category { CategoryId = 1, Name = "Senior", EventId = 1 });
        await db.SaveChangesAsync();

        var controller = WithUser(new EnrolmentsController(db), 1, "Participant");
        var result = await controller.Enrol(new EnrolmentCreateDto { EventId = 1, CategoryId = 1 });

        Assert.IsType<OkObjectResult>(result);
        Assert.Single(db.Enrolments);
    }

    [Fact]
    public async Task Enrol_Duplicate_ReturnsBadRequest()
    {
        var db = GetDb();
        db.Users.Add(new User { UserId = 1, Name = "P", Email = "p@t.com", PasswordHash = "h", Role = "Participant" });
        db.Events.Add(new Event { EventId = 1, Title = "E", Description = "D", Date = DateTime.UtcNow, Location = "L", Distance = 10, EventType = "Run", OrganiserId = 2 });
        db.Categories.Add(new Category { CategoryId = 1, Name = "Senior", EventId = 1 });
        db.Enrolments.Add(new Enrolment { ParticipantId = 1, EventId = 1, CategoryId = 1, Status = "Pending" });
        await db.SaveChangesAsync();

        var controller = WithUser(new EnrolmentsController(db), 1, "Participant");
        var result = await controller.Enrol(new EnrolmentCreateDto { EventId = 1, CategoryId = 1 });
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Enrol_CategoryNotInEvent_ReturnsBadRequest()
    {
        var db = GetDb();
        db.Users.Add(new User { UserId = 1, Name = "P", Email = "p@t.com", PasswordHash = "h", Role = "Participant" });
        db.Events.Add(new Event { EventId = 1, Title = "E", Description = "D", Date = DateTime.UtcNow, Location = "L", Distance = 10, EventType = "Run", OrganiserId = 2 });
        db.Events.Add(new Event { EventId = 2, Title = "E2", Description = "D", Date = DateTime.UtcNow, Location = "L", Distance = 5, EventType = "Walk", OrganiserId = 2 });
        db.Categories.Add(new Category { CategoryId = 1, Name = "Senior", EventId = 2 });
        await db.SaveChangesAsync();

        var controller = WithUser(new EnrolmentsController(db), 1, "Participant");
        var result = await controller.Enrol(new EnrolmentCreateDto { EventId = 1, CategoryId = 1 });
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
