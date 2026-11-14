using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class TicketController : Controller
    {
        private readonly ApiService _api;

        public TicketController(ApiService apiService)
        {
            _api = apiService;
        }

        
        public async Task<IActionResult> Index(string? ticketType)
        {
           
            var url = "api/ticket" + (string.IsNullOrEmpty(ticketType) ? "" : $"?ticketType={ticketType}");
            
           
            var events = await _api.GetAsync<List<TicketViewModel>>(url) ?? new();
            
            
            ViewBag.TicketType = ticketType;

            
            return View(events);
        }
    }
}