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

   
    public async Task<IActionResult> Index(string? eventType)
    {
        var url = "api/events" + (string.IsNullOrEmpty(eventType) ? "" : $"?eventType={eventType}");
        var events = await _api.GetAsync<List<EventViewModel>>(url) ?? new();
        ViewBag.EventType = eventType;
        return View(events);
    }

    
    public async Task<IActionResult> Details(int id)
    {
        var ev = await _api.GetAsync<EventViewModel>($"api/events/{id}");
        if (ev == null) return NotFound();
        return View(ev);
    }

   
    [SessionAuthorize(Role = "Participant")]
    [HttpPost]
    public async Task<IActionResult> Enrol(int eventId, int categoryId)
    {
        var (success, _) = await _api.PostAsync("api/enrolments",
            new { EventId = eventId, CategoryId = categoryId });

        TempData[success ? "Success" : "Error"] = success
            ? "Enrolment submitted!"
            : "Enrolment failed (may already exist).";

        return RedirectToAction(success ? "MyEnrolments" : "Details", success ? null : new { id = eventId });
    }


    [SessionAuthorize(Role = "Participant")]
    public async Task<IActionResult> MyEnrolments()
    {
        var enrolments = await _api.GetAsync<List<EnrolmentViewModel>>("api/enrolments/my-enrolments") ?? new();
        return View(enrolments);
    }

   
    [SessionAuthorize(Role = "Participant")]
    public async Task<IActionResult> MyResults()
    {
        var results = await _api.GetAsync<List<ResultViewModel>>("api/results/my-results") ?? new();
        return View(results);
    }
}