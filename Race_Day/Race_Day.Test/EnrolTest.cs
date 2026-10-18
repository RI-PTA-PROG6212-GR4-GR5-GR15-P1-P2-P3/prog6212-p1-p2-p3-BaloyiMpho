
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Race_Day.Controllers;
using Race_Day.Data;
using Race_Day.Models;
using Race_Day.Models.DTOs;
using Race_Day.Test.TestHelpers;
using Xunit;

namespace Race_Day.Test
{
    public class EnrolTest
    {

        [Fact]
        public async Task Enrol_ShouldReturnOk() 
        {
            // Arrange
            var (controller, db) = TestFactory.CreateEnrolmentController();

           
            var testUserId = Guid.NewGuid();
            var testCategoryId = Guid.NewGuid();
            var testEventId = Guid.NewGuid();
            var testUser = new User
            {
                UserId = testUserId,
                Username = "testuser123",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateOnly(1990, 1, 1),
                PhoneNumber = "0821234567",
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                IsActive = true
            };

            var testEvent = new Event
            {
                EventId = testEventId,
                EventName = "Test Marathon",
                EventDate = DateOnly.FromDateTime(DateTime.Today),
                Location = "Test Location",
                Venue = "Test Venue",
                DistanceKm = 10.0f,
                EventType = "Marathon",
                EventFee = 100.0f,
                Status = "Scheduled",
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Now = DateTime.UtcNow
            };
            var testCategory = new Category
            {
                CategoryId = testCategoryId,
                EventId = testEventId,
                CategoryName = "Senior Men",
                MinAge = 18,
                MaxAge = 39,
                GenderRestrictions = "Male",
                MaxParticipants = 100,
                CreatedBy = "admin_user",
                CreatedAt = DateTime.UtcNow
            };

            db.users.Add(testUser);
            db.categories.Add(testCategory);
            db.events.Add(testEvent);
            await db.SaveChangesAsync();

           
            var dto = new NewEnrol
            {
                UserId = testUserId,
                CategoryId = testCategoryId,
                EventId = testEventId,
                amount_paid = 0.0m,
                payment_status = "Paid",
                paid_date = DateOnly.FromDateTime(DateTime.UtcNow),
                enrolment_date = DateOnly.FromDateTime(DateTime.UtcNow),
                enrolment_status = "Confirmed",
                



            };

            // Act
            var result = controller.EnrolUserInCategory(testUserId, testCategoryId);
          

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
       
        public async Task EditEnrolment_WhenUserIsAdmin_ReturnsSuccess()
        {
            // Arrange
            var (controller, db) = TestFactory.CreateEnrolmentController();

            // Mock the Admin User
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Name, "admin_user"),
        new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

           
            var existingEnrolmentId = Guid.NewGuid();

            var existingEnrolment = new Enrolment
            {
                EnrolmentId = existingEnrolmentId,
                UserId = Guid.NewGuid(), 
                EventId = Guid.NewGuid(), 
                CategoryId = Guid.NewGuid(), 
                enrolment_date = DateOnly.FromDateTime(DateTime.Today),
                enrolment_status = "Pending",
                payment_status = "Unpaid",
                amount_paid = 0.0m,
                paid_date = DateOnly.FromDateTime(DateTime.Today)
            };

            db.enrolments.Add(existingEnrolment);
            await db.SaveChangesAsync();

            var dto = new UpdateEnrolmentDTO
            {
                enrolment_date = DateOnly.FromDateTime(DateTime.Today),
                enrolment_status = "Done",
                payment_status = "Paid",
                amount_paid = 0.0m,
                paid_date = DateOnly.FromDateTime(DateTime.Today)
            };

            
            var result =  controller.PatchEnrolment(existingEnrolmentId, dto);

            // Assert
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);

           
        }

        [Fact]
        public async Task EditEnrolment_WhenUserIsNotAdmin_ShouldBeDenied()
        {
            // Arrange
            var (controller, db) = TestFactory.CreateEnrolmentController();

            // Mock the Normal User
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "normal_user"),
                new Claim(ClaimTypes.Role, "Participant")
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var dto = new UpdateEnrolmentDTO 
            {
                enrolment_date = DateOnly.FromDateTime(DateTime.Today),
                enrolment_status = "Pending",
                payment_status = "Unpaid",
                amount_paid = 0.0m,
                paid_date = DateOnly.FromDateTime(DateTime.Today)
            };

            // Act
            var result =  controller.PatchEnrolment(Guid.NewGuid(), dto);

            // Assert
            
            var forbidResult = Assert.IsType<ForbidResult>(result);
        }

      

        [Fact]
        public void Create_NewEnrolment_ShouldHaveValidData()
        {
            // Arrange
            var testUserId = Guid.NewGuid();
            var testEventId = Guid.NewGuid();
            var testCategoryId = Guid.NewGuid();

            var testEnrolment = new Enrolment
            {
                UserId = testUserId,
                EventId = testEventId,
                CategoryId = testCategoryId,
                enrolment_date = DateOnly.FromDateTime(DateTime.Today),
                enrolment_status = "Confirmed",
                payment_status = "Paid",
                amount_paid = 150.00m, // 'm' for decimal (Money should always be decimal)
                paid_date = DateOnly.FromDateTime(DateTime.Today),

            };

            
            Assert.NotNull(testEnrolment);
            Assert.Equal("Confirmed", testEnrolment.enrolment_status);
            Assert.Equal(150.00m, testEnrolment.amount_paid);
            Assert.Equal(testUserId, testEnrolment.UserId);
        }
    }
}