using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using System;

namespace RaceDay.API.Services
{
    public interface ITicketNumberGenerator
    {
        Task<string> GenerateAsync(int eventId, CancellationToken ct = default);
    }

    public class TicketNumberGenerator : ITicketNumberGenerator
    {
        private readonly RaceDayDbContext _db;

        public TicketNumberGenerator(RaceDayDbContext db) => _db = db;

        public async Task<string> GenerateAsync(int eventId, CancellationToken ct = default)
        {
           
            var count = await _db.Tickets.CountAsync(t => t.EventId == eventId, ct);
           
            return $"EVT-{eventId}-{count + 1:D6}";
        }
    }
}