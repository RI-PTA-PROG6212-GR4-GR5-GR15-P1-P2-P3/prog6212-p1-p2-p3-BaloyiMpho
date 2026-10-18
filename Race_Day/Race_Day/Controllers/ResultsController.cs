using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Race_Day.Data;
using Race_Day.Models.DTOs;

namespace Race_Day.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbcontext _context;
        public ResultsController(ApplicationDbcontext dbContext)
        {
            _context = dbContext;
        }
        [HttpGet]
        public IActionResult GetResults()
        {
            var results = _context.results.ToList();
                return Ok(new { message = "Results retrieved successfully.",results });
        }
        public IActionResult UpdateResult(Guid resultId, [FromBody] editResultDTO updateResultDTO)
        {
            var existingResult = _context.results.FirstOrDefault(r => r.ResultId == resultId);
            if (existingResult == null)
            {
                return NotFound(new { message = "Result not found." });
            }
            existingResult.ParticipantEventId = updateResultDTO.ParticipantEventId;
            existingResult.finish_position = updateResultDTO.finish_position;
            existingResult.finish_time_seconds = updateResultDTO.finish_time_seconds;
            existingResult.pace_pace_km = updateResultDTO.pace_pace_km;
            existingResult.overall_rank = updateResultDTO.overall_rank;
            existingResult.prize_money = updateResultDTO.prize_money;
            existingResult.category_rank = updateResultDTO.category_rank;
            _context.SaveChanges();
            return Ok(new { message = "Result updated successfully.", updatedResult = existingResult });
        }
        [HttpGet]
        public IActionResult GetResultById(Guid resultId)
        {
            var result = _context.results.FirstOrDefault(r => r.ResultId == resultId);
            if (result == null)
            {
                return NotFound(new { message = "Result not found." });
            }
            return Ok(new { message = "Result retrieved successfully.", result });
        }
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteResult(Guid resultId)
        {
            var result = _context.results.FirstOrDefault(r => r.ResultId == resultId);
            if (result == null)
            {
                return NotFound(new { message = "Result not found." });
            }
            _context.results.Remove(result);
            _context.SaveChanges();
            return Ok(new { message = "Result deleted successfully." });
        }

        [HttpGet]
        public IActionResult GetLeaderBoard() 
        {
            var leaderboard = _context.results
                .OrderBy(r => r.overall_rank)
                .Select(r => new
                {
                    r.ResultId,
                    r.ParticipantEventId,
                    r.finish_position,
                    r.finish_time_seconds,
                    r.pace_pace_km,
                    r.overall_rank,
                    r.prize_money,
                    r.category_rank
                })
                .ToList();
            return Ok(new { message = "Leaderboard retrieved successfully.", leaderboard });
        }
    }
}
