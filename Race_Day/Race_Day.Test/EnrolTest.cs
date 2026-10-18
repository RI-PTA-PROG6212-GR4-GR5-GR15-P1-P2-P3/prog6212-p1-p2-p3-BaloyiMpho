
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
        public void Enrol_ShouldReturnOk()
        {
            // Arrange
            var (controller, db) = TestFactory.CreateEnrolmentController();

            var dto = new Enrolment
            {
                UserId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                enrolment_date = DateOnly.FromDateTime(DateTime.UtcNow),
                amount_paid = 0.0m,

                // server-controlled
                enrolment_status = "Pending",
                payment_status = "Paid",
                paid_date = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            // Act
            var result =  controller.EnrolUserInCategory(Guid.NewGuid(),dto.UserId);

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

            var dto = new UpdateEnrolmentDTO
            {
                enrolment_date = DateOnly.FromDateTime(DateTime.Today),
                enrolment_status = "Done",
                payment_status = "Paid",
                amount_paid = 0.0m,
                paid_date = DateOnly.FromDateTime(DateTime.Today)
            };

            // Act
            var result =  controller.PatchEnrolment(Guid.NewGuid(), dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            
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
            // Note: If your controller returns UnauthorizedResult, change this to UnauthorizedResult
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
