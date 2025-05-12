using apiTechSkillPro.Data;
using apiTechSkillPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiTechSkillPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizAttemptLogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuizAttemptLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/QuizAttemptLog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizAttemptLog>>> GetLogs()
        {
            return await _context.QuizAttemptLogs
                .Include(l => l.Question)
                .Include(l => l.QuizAttempt1)
                .ToListAsync();
        }

        // GET: api/QuizAttemptLog/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QuizAttemptLog>> GetLog(int id)
        {
            var log = await _context.QuizAttemptLogs
                .Include(l => l.Question)
                .Include(l => l.QuizAttempt1)
                .FirstOrDefaultAsync(l => l.LogID == id);

            if (log == null)
                return NotFound();

            return log;
        }

        // GET: api/QuizAttemptLog/byAttempt/3
        [HttpGet("byAttempt/{attemptId}")]
        public async Task<ActionResult<IEnumerable<QuizAttemptLog>>> GetLogsByAttempt(int attemptId)
        {
            return await _context.QuizAttemptLogs
                .Where(l => l.AttemptID == attemptId)
                .Include(l => l.Question)
                .ToListAsync();
        }

        // POST: api/QuizAttemptLog
        [HttpPost]
        public async Task<ActionResult<QuizAttemptLog>> PostLog(QuizAttemptLog log)
        {
            _context.QuizAttemptLogs.Add(log);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLog), new { id = log.LogID }, log);
        }

        // DELETE: api/QuizAttemptLog/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLog(int id)
        {
            var log = await _context.QuizAttemptLogs.FindAsync(id);
            if (log == null)
                return NotFound();

            _context.QuizAttemptLogs.Remove(log);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
