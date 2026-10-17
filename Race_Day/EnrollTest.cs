using Race_Day.Models.DTOs;

namespace Race_Day.Test
{
    public class EventTest
    {
        [Fact]
        public void NewEnroll_ShouldReturnOk()
        {
            var var(controller, db) = TestFactory.CreateAuthController();
            var dto = new NewEnrol
            {
                UserId = testUserId,
                EventId = testEventId,
                CategoryId = testCategoryId,
                enrolment_date = DateOnly.FromDateTime(DateTime.Today),
                enrolment_status = "Confirmed",
                payment_status = "Paid",
                amount_paid = 150.00m, 
                paid_date = DateOnly.FromDateTime(DateTime.Today),


            };
            var result = await controller.Enrolment(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public  void CreateEvent_WhenUserIsAdmin_ReturnsSuccess() 
        {
            var (controller, db) = TestFactory.CreateAuthController();

           
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "admin_user"),
            new Claim(ClaimTypes.Role, "Admin") 
            }, "mock"));
           
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var dto = new NewEventDTO
            {
                EventName = "Women Only Marathon",
                EventDate = DateOnly.FromDateTime(DateTime.Today),
                Location = "Johannesburg",
                Venue = "FNB Stadium",
                DistanceKm = 42.2f,
                EventType = "Marathon",
                EventFee = 250.0m, 
                Status = "Pending",
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

           
            var savedEvent = await db.Events.FirstOrDefaultAsync(e => e.EventName == "Women Only Marathon");
            Assert.NotNull(savedEvent);
        }
        [Fact]
        public void CreateEvent_WhenUserIsNotAdmin_ShouldBeDenied() 
        {
            var (controller, db) = TestFactory.CreateAuthController();

            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "normal_user"),
            new Claim(ClaimTypes.Role, "Participant") // <--- Not an Admin
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            //Act
            var dto = new NewEventDTO { EventName = "Unauthorized Race"};
            var result = await controller.CreateEvent(dto);
            
            //Assert
            var badRequest = Assert.IsType<ForbidResult>(result);
        }
    }
}