using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests;

public class AuthControllerTests
{
    private static RaceDayDbContext GetDb()
    {
        var opts = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new RaceDayDbContext(opts);
    }

    private static IConfiguration GetConfig() =>
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "TestKeyForUnitTestingMustBe32Chars!",
            ["Jwt:Issuer"] = "test",
            ["Jwt:Audience"] = "test",
            ["Jwt:ExpiryHours"] = "3"
        }).Build();

    
    private static IPasswordHasher<User> GetHasher() => new PasswordHasher<User>();

    [Fact]
    public async Task Register_NewUser_ReturnsOk()
    {
        var db = GetDb();
        var hasher = GetHasher();
        var controller = new AuthController(db, GetConfig(), hasher);

        var result = await controller.Register(new RegisterDto
        {
            Name = "Test",
            Email = "test@test.com",
            Password = "Pass123!",
            Role = "Participant"
        });

        Assert.IsType<OkObjectResult>(result);
        Assert.Single(db.Users);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        var db = GetDb();
        var hasher = GetHasher();

        db.Users.Add(new User { Name = "Ex", Email = "test@test.com", PasswordHash = "hash", Role = "Participant" });
        await db.SaveChangesAsync();

        var controller = new AuthController(db, GetConfig(), hasher);
        var result = await controller.Register(new RegisterDto
        {
            Name = "Test",
            Email = "test@test.com",
            Password = "Pass123!",
            Role = "Participant"
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        var db = GetDb();
        var hasher = GetHasher();

       
        var testUser = new User { Name = "Test", Email = "test@test.com", Role = "Participant" };
        testUser.PasswordHash = hasher.HashPassword(testUser, "Pass123!");

        db.Users.Add(testUser);
        await db.SaveChangesAsync();

        var controller = new AuthController(db, GetConfig(), hasher);
        var result = await controller.Login(new LoginDto { Email = "test@test.com", Password = "Pass123!" });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        var db = GetDb();
        var hasher = GetHasher();

        var testUser = new User { Name = "Test", Email = "test@test.com", Role = "Participant" };
        testUser.PasswordHash = hasher.HashPassword(testUser, "Pass123!");

        db.Users.Add(testUser);
        await db.SaveChangesAsync();

        var controller = new AuthController(db, GetConfig(), hasher);
        var result = await controller.Login(new LoginDto { Email = "test@test.com", Password = "WrongPassword!" });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_NonExistentEmail_ReturnsUnauthorized()
    {
        var db = GetDb();
        var hasher = GetHasher();
        var controller = new AuthController(db, GetConfig(), hasher);

        var result = await controller.Login(new LoginDto { Email = "no@test.com", Password = "Pass123!" });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}