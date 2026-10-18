using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Race_Day.Models;
using Race_Day.Models.DTOs;
using Race_Day.Test.TestHelpers;

namespace Race_Day.Test
{
    public class EventTest
    {
        [Fact]
        public void Create_NewEvent_ShouldReturnOk()
        {
            var (controller, db) = TestFactory.CreateEventController();
            var dto = new EnrolmentDTO
            {
                EventName = "ABC Run fest",
                EventDate = DateOnly.FromDateTime(DateTime.UtcNow)  ,
                Now =DateTime.UtcNow   ,
                Location = "Gauteng Springs",
                Venue = "Springs Mall Entrance 5",
                DistanceKm = (float)100.0m,
                EventType = "Womens Month Rush",
                EventFee = (float)120.0m,
                Status = "Pending",
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,

            };
            var result =  controller.CreateEvent(dto);
            var okResult = Assert.IsType<CreatedAtActionResult>(result);
        }
        [Fact]
        public  void CreateEvent_WhenUserIsAdmin_ReturnsSuccess() 
        {
            var (controller, db) = TestFactory.CreateEventController();

           
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "admin_user"),
            new Claim(ClaimTypes.Role, "Admin") 
            }, "mock"));
           
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var dto = new EnrolmentDTO
            {
                EventName = "Women Only Marathon",
                EventDate = DateOnly.FromDateTime(DateTime.Today),
                Location = "Johannesburg",
                Venue = "FNB Stadium",
                DistanceKm = 42.2f,
                EventType = "Marathon",
                EventFee = 250.0f, 
                Status = "Pending",
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Now = DateTime.UtcNow,
            };
            var result =  controller.CreateEvent(dto);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);


            Assert.NotNull(createdAtResult.Value);


        }
        
        [Fact]
        public void EditEvent_WhenUserIsNotAdmin_ShouldBeDenied()
        {
            // Arrange
            var (controller, db) = TestFactory.CreateEventController();

            // 1. Create the event and save it to the DB
            var existingEventId = Guid.NewGuid();
            var existingEvent = new Event
            {
                EventId = existingEventId,
                EventName = "Original Name",
                EventDate = DateOnly.FromDateTime(DateTime.Today),
                Location = "Original Location",
                Venue = "Original Venue",
                DistanceKm = 10.0f,
                EventType = "Original Type",
                EventFee = 100.0f,
                Status = "Scheduled",
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Now = DateTime.UtcNow
            };

            db.events.Add(existingEvent);
            db.SaveChanges();

            // 2. Mock the Non-Admin User
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Name, "normal_user"),
        new Claim(ClaimTypes.Role, "Participant") // Not Admin or Organiser
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // 3. Act
            var dto = new UpdateEventDTO
            {
                EventName = "Absa Trend Run",
                EventDate = DateOnly.FromDateTime(DateTime.Today),
                Location = "JHB",
                Venue = "Soweto",
                DistanceKm = 42.2f,
                EventType = "Short Marathon",
                EventFee = 250.0f,
                Status = "Pending",
                Now = DateTime.UtcNow,
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = controller.UpdateEvent(existingEventId, dto);

            // 4. Assert
            
            var forbidResult = Assert.IsType<ForbidResult>(result);
        }
    }
}