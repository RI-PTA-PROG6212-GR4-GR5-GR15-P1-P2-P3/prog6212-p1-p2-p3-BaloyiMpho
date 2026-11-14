using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Attributes;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers;

[SessionAuthorize(Role = "Participant")]
public class PaymentController : Controller
{
    private readonly ApiService _api;

    public PaymentController(ApiService api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int ticketId)
    {
        var tickets = await _api.GetAsync<List<TicketMineViewModel>>("api/tickets/mine") ?? new();
        var ticket = tickets.FirstOrDefault(t => t.Id == ticketId);

        if (ticket == null)
        {
            TempData["Error"] = "Ticket not found.";
            return RedirectToAction("MyEnrolments", "Events");
        }

        var model = new PaymentFormViewModel
        {
            TicketId = ticketId,
            TicketNumber = ticket.TicketNumber,
            EventName = ticket.EventTitle,
            CategoryName = ticket.CategoryName,
            TicketType = ticket.TicketType,
            Amount = ticket.Price,
            PaymentStatus = ticket.PaymentStatus
        };

        return View("Payment", model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(int ticketId, decimal amount)
    {
        var (success, error, data) = await _api.PostAsync<PaymentResult>(
            "api/payments",
            new { TicketId = ticketId, Amount = amount });

        if (success && data != null)
        {
            TempData["Success"] = string.IsNullOrEmpty(data.Message)
                ? "Payment completed successfully."
                : data.Message;

            return RedirectToAction("MyTickets", "Events");
        }

        TempData["Error"] = string.IsNullOrEmpty(error)
            ? "Payment failed."
            : error;

        return RedirectToAction("Pay", new { ticketId, amount });
    }
}
