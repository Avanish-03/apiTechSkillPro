using apiTechSkillPro.Data;
using apiTechSkillPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTechSkillPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizAttemptController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuizAttemptController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/QuizAttempt
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizAttempt>>> GetQuizAttempts()
        {
            return await _context.QuizAttempts
                .Include(a => a.User)
                .Include(a => a.Quiz)
                .ToListAsync();
        }

        // GET: api/QuizAttempt/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QuizAttempt>> GetQuizAttempt(int id)
        {
            var attempt = await _context.QuizAttempts
                .Include(a => a.User)
                .Include(a => a.Quiz)
                .Include(a => a.Answers)
                .FirstOrDefaultAsync(a => a.AttemptID == id);

            if (attempt == null)
                return NotFound();

            return attempt;
        }

        // POST: api/QuizAttempt
        [HttpPost]
        public async Task<ActionResult<QuizAttempt>> StartAttempt(QuizAttempt attempt)
        {
            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuizAttempt), new { id = attempt.AttemptID }, attempt);
        }

        // PUT: api/QuizAttempt/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttempt(int id, QuizAttempt updatedAttempt)
        {
            if (id != updatedAttempt.AttemptID)
                return BadRequest();

            _context.Entry(updatedAttempt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.QuizAttempts.Any(a => a.AttemptID == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/QuizAttempt/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttempt(int id)
        {
            var attempt = await _context.QuizAttempts.FindAsync(id);
            if (attempt == null)
                return NotFound();

            _context.QuizAttempts.Remove(attempt);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
