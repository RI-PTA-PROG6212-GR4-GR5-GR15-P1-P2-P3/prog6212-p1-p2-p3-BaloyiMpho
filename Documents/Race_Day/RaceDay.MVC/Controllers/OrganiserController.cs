using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Attributes;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers;

[SessionAuthorize(Role = "Admin")]
public class OrganiserController : Controller
{
    private readonly ApiService _api;
    public OrganiserController(ApiService api) => _api = api;

    public async Task<IActionResult> Dashboard()
    {
        var events = await _api.GetAsync<List<EventViewModel>>("api/events/my-events") ?? new();
        return View(events);
    }

    [HttpGet]
    public IActionResult CreateEvent() => View(new CreateEventViewModel());

    [HttpPost]
    public async Task<IActionResult> CreateEvent(CreateEventViewModel model)
    {
        var form = new MultipartFormDataContent();
        form.Add(new StringContent(model.Title), "Title");
        form.Add(new StringContent(model.Description), "Description");
        form.Add(new StringContent(model.Date.ToString("yyyy-MM-dd")), "Date");
        form.Add(new StringContent(model.Location), "Location");
        form.Add(new StringContent(model.Distance.ToString()), "Distance");
        form.Add(new StringContent(model.EventType), "EventType");

        if (!string.IsNullOrWhiteSpace(model.CategoriesCsv))
        {
            var cats = model.CategoriesCsv.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < cats.Length; i++)
                form.Add(new StringContent(cats[i]), $"Categories[{i}]");
        }

        if (model.BannerImage != null && model.BannerImage.Length > 0)
        {
            var stream = model.BannerImage.OpenReadStream();
            var file = new StreamContent(stream);
            file.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.BannerImage.ContentType);
            form.Add(file, "BannerImage", model.BannerImage.FileName);
        }

        var (success, error) = await _api.PostFormAsync("api/events", form);

        if (!success)
        {
           
            ModelState.AddModelError("", $"API Error: {error ?? "Unknown error"}");
            return View(model);
        }
        TempData["Success"] = "Event created!";
        return RedirectToAction("Dashboard");
    }

    public async Task<IActionResult> EventEnrolments(int eventId)
    {
        var enrolments = await _api.GetAsync<List<EnrolmentViewModel>>($"api/enrolments/event/{eventId}") ?? new();
        ViewBag.EventId = eventId;
        return View(enrolments);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmEnrolment(int enrolmentId, int eventId)
    {
        var (success, _) = await _api.PutEmptyAsync($"api/enrolments/{enrolmentId}/confirm");
        TempData[success ? "Success" : "Error"] = success ? "Enrolment confirmed." : "Failed.";
        return RedirectToAction("EventEnrolments", new { eventId });
    }

    public async Task<IActionResult> EventResults(int eventId)
    {
        var results = await _api.GetAsync<List<ResultViewModel>>($"api/results/event/{eventId}") ?? new();
        var enrolments = await _api.GetAsync<List<EnrolmentViewModel>>($"api/enrolments/event/{eventId}") ?? new();
        ViewBag.EventId = eventId;
        ViewBag.Enrolments = enrolments;
        return View(results);
    }

    [HttpPost]
    public async Task<IActionResult> CaptureResult(int eventId, int participantId, string finishTime, int position)
    {
        if (!TimeSpan.TryParse(finishTime, out var ts))
        {
            TempData["Error"] = "Invalid finish time format (use hh:mm:ss).";
            return RedirectToAction("EventResults", new { eventId });
        }

        var (success, _) = await _api.PostAsync("api/results", new
        {
            EventId = eventId, ParticipantId = participantId,
            FinishTime = ts, FinishingPosition = position
        });
        TempData[success ? "Success" : "Error"] = success ? "Result recorded." : "Failed to record result.";
        return RedirectToAction("EventResults", new { eventId });
    }
}
