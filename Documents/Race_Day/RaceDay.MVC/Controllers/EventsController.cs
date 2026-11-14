using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Attributes;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers;

[SessionAuthorize]
public class EventsController : Controller
{
    private readonly ApiService _api;

    public EventsController(ApiService api) => _api = api;

    private static string FriendlyApiError(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "Enrolment failed.";

        try
        {
            using var doc = JsonDocument.Parse(raw);
            if (doc.RootElement.TryGetProperty("message", out var message))
                return message.GetString() ?? raw;
            if (doc.RootElement.TryGetProperty("error", out var error))
                return error.GetString() ?? raw;
            if (doc.RootElement.TryGetProperty("title", out var title))
                return title.GetString() ?? raw;
            if (doc.RootElement.TryGetProperty("detail", out var detail))
                return detail.GetString() ?? raw;
        }
        catch
        {
            // raw is not JSON; return as-is.
        }

        return raw.Trim();
    }

    public async Task<IActionResult> Index(string? eventType)
    {
        var url = "api/events" + (string.IsNullOrEmpty(eventType) ? "" : $"?eventType={eventType}");
        var events = await _api.GetAsync<List<EventViewModel>>(url) ?? new();
        ViewBag.EventType = eventType;
        return View(events);
    }

    public async Task<IActionResult> Details(int id)
    {
        if (id == 0) return Content("Error: The Event ID was 0.");
        var ev = await _api.GetAsync<EventViewModel>($"api/events/{id}");
        if (ev == null) return NotFound();
        return View(ev);
    }

    [SessionAuthorize(Role = "Participant")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enrol(int eventId, int categoryId, string ticketType = "Standard")
    {
        if (eventId == 0)
        {
            TempData["Error"] = "Invalid event.";
            return RedirectToAction("Index");
        }

        var (success, error, data) = await _api.PostAsync<EnrolmentResult>(
            "api/enrolments",
            new { EventId = eventId, CategoryId = categoryId, TicketType = ticketType });

        if (success && data != null && data.Ticket != null)
        {
            // API returns "id" on the ticket; TicketId is never populated.
            var ticketId = data.Ticket.Id;

            if (ticketId <= 0)
            {
                TempData["Error"] = "Enrolment succeeded but no ticket was returned. Please check My Enrolments.";
                return RedirectToAction("MyEnrolments");
            }

            TempData["Success"] = data.Message;
            return RedirectToAction("Confirmation",
                new { enrolmentId = data.EnrolmentId, ticketId });
        }

        Console.WriteLine(
            $"API enrolment failed. Status: {(success ? "success" : "error")}. " +
            $"Raw response: {error ?? "<empty>"}. " +
            $"EventId={eventId}, CategoryId={categoryId}, TicketType={ticketType}.");

        TempData["Error"] = FriendlyApiError(error);
        return RedirectToAction("Details", new { id = eventId });
    }

    [SessionAuthorize(Role = "Participant")]
    public async Task<IActionResult> Confirmation(int enrolmentId, int ticketId)
    {
        var enrolments = await _api.GetAsync<List<EnrolmentViewModel>>("api/enrolments/my-enrolments") ?? new();
        var enrolment = enrolments.FirstOrDefault(e => e.Id == enrolmentId);

        if (enrolment == null)
        {
            TempData["Error"] = "Enrolment not found.";
            return RedirectToAction("MyEnrolments");
        }

        var eventDetails = await _api.GetAsync<EventViewModel>($"api/events/{enrolment.EventId}");

        var ticket = new TicketConfirmationViewModel
        {
            EnrolmentId = enrolmentId,
            TicketId = ticketId,
            EventName = eventDetails?.Title ?? enrolment.EventTitle,
            CategoryName = enrolment.CategoryName,
            Location = eventDetails?.Location ?? "",
            TicketType = "Standard",
            Price = 0,
            PaymentStatus = "Pending",
            Status = enrolment.Status,
            Message = TempData["Success"] as string ?? "Enrolment submitted. Awaiting payment."
        };

        if (eventDetails != null)
        {
            var selectedCategory = eventDetails.Categories
                .FirstOrDefault(c => c.Name == enrolment.CategoryName);
            if (selectedCategory != null)
            {
                ticket.CategoryName = selectedCategory.Name;
            }
        }

        var myTickets = await _api.GetAsync<List<TicketMineViewModel>>("api/tickets/mine") ?? new();
        var myTicket = myTickets.FirstOrDefault(t => t.Id == ticketId);
        if (myTicket != null)
        {
            ticket.TicketNumber = myTicket.TicketNumber;
            ticket.TicketType = myTicket.TicketType;
            ticket.Price = myTicket.Price;
            ticket.PaymentStatus = myTicket.PaymentStatus;
        }

        return View(ticket);
    }

    [SessionAuthorize(Role = "Participant")]
    public async Task<IActionResult> MyEnrolments()
    {
        var enrolments = await _api.GetAsync<List<EnrolmentViewModel>>("api/enrolments/my-enrolments") ?? new();
        return View(enrolments);
    }

    [SessionAuthorize(Role = "Participant")]
    public async Task<IActionResult> MyTickets()
    {
        var tickets = await _api.GetAsync<List<TicketMineViewModel>>("api/tickets/mine") ?? new();
        return View(tickets);
    }

    [SessionAuthorize(Role = "Participant")]
    public async Task<IActionResult> MyResults()
    {
        var results = await _api.GetAsync<List<ResultViewModel>>("api/results/my-results") ?? new();
        return View(results);
    }
}