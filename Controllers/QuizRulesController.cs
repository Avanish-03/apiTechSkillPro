using apiTechSkillPro.Data;
using apiTechSkillPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiTechSkillPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizRulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuizRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/QuizRules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizRules>>> GetRules()
        {
            return await _context.QuizRules
                .Include(r => r.Quiz)
                .ToListAsync();
        }

        // GET: api/QuizRules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QuizRules>> GetRule(int id)
        {
            var rule = await _context.QuizRules
                .Include(r => r.Quiz)
                .FirstOrDefaultAsync(r => r.RuleID == id);

            if (rule == null)
                return NotFound();

            return rule;
        }

        // GET: api/QuizRules/quiz/3
        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<QuizRules>> GetRuleByQuizId(int quizId)
        {
            var rule = await _context.QuizRules
                .FirstOrDefaultAsync(r => r.QuizID == quizId);

            if (rule == null)
                return NotFound();

            return rule;
        }

        // POST: api/QuizRules
        [HttpPost]
        public async Task<ActionResult<QuizRules>> PostRule(QuizRules rule)
        {
            _context.QuizRules.Add(rule);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRule), new { id = rule.RuleID }, rule);
        }

        // PUT: api/QuizRules/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRule(int id, QuizRules rule)
        {
            if (id != rule.RuleID)
                return BadRequest();

            _context.Entry(rule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.QuizRules.Any(e => e.RuleID == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/QuizRules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRule(int id)
        {
            var rule = await _context.QuizRules.FindAsync(id);
            if (rule == null)
                return NotFound();

            _context.QuizRules.Remove(rule);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
