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
    public class EnrolTest
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
            var okResult = Assert.IsType<OkObjectResult>(result);
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

            var okResult = Assert.IsType<OkObjectResult>(result);


            Assert.NotNull(okResult.Value);

           

        }
        [Fact]
        public void CreateEvent_WhenUserIsNotAdmin_ShouldBeDenied() 
        {
            var (controller, db) = TestFactory.CreateEventController();

            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "normal_user"),
            new Claim(ClaimTypes.Role, "Participant") 
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            //Act
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
            
            //Assert
            var badRequest = Assert.IsType<ForbidResult>(result);
        }
    }
}
