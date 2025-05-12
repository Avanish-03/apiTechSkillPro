using apiTechSkillPro.Data;
using apiTechSkillPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiTechSkillPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResultController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Result
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Result>>> GetResults()
        {
            return await _context.Results
                .Include(r => r.User)
                .Include(r => r.Quiz)
                .Include(r => r.QuizAttempt)
                .ToListAsync();
        }

        // GET: api/Result/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Result>> GetResult(int id)
        {
            var result = await _context.Results
                .Include(r => r.User)
                .Include(r => r.Quiz)
                .Include(r => r.QuizAttempt)
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null)
                return NotFound();

            return result;
        }

        // GET: api/Result/user/3
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Result>>> GetResultsByUser(int userId)
        {
            return await _context.Results
                .Include(r => r.Quiz)
                .Where(r => r.UserID == userId)
                .ToListAsync();
        }

        // GET: api/Result/quiz/2
        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<IEnumerable<Result>>> GetResultsByQuiz(int quizId)
        {
            return await _context.Results
                .Include(r => r.User)
                .Where(r => r.QuizID == quizId)
                .ToListAsync();
        }

        // POST: api/Result
        [HttpPost]
        public async Task<ActionResult<Result>> PostResult(Result result)
        {
            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetResult), new { id = result.ResultID }, result);
        }

        // PUT: api/Result/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResult(int id, Result result)
        {
            if (id != result.ResultID)
                return BadRequest();

            _context.Entry(result).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Results.Any(e => e.ResultID == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Result/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null)
                return NotFound();

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
