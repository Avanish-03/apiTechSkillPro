using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiTechSkillPro.Data;
using apiTechSkillPro.Models;

namespace apiTechSkillPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProgressController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserProgressController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UserProgress
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProgress>>> GetUserProgress()
        {
            return await _context.UserProgresses
                .Include(up => up.User)
                .Include(up => up.Quiz)
                .Include(up => up.LastAttempt)
                .ToListAsync();
        }

        // GET: api/UserProgress/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProgress>> GetUserProgress(int id)
        {
            var userProgress = await _context.UserProgresses
                .Include(up => up.User)
                .Include(up => up.Quiz)
                .Include(up => up.LastAttempt)
                .FirstOrDefaultAsync(up => up.ProgressID == id);

            if (userProgress == null)
                return NotFound();

            return userProgress;
        }

        // PUT: api/UserProgress/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserProgress(int id, UserProgress userProgress)
        {
            if (id != userProgress.ProgressID)
                return BadRequest();

            _context.Entry(userProgress).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UserProgresses.Any(e => e.ProgressID == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/UserProgress
        [HttpPost]
        public async Task<ActionResult<UserProgress>> PostUserProgress(UserProgress userProgress)
        {
            _context.UserProgresses.Add(userProgress);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserProgress), new { id = userProgress.ProgressID }, userProgress);
        }

        // DELETE: api/UserProgress/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserProgress(int id)
        {
            var userProgress = await _context.UserProgresses.FindAsync(id);
            if (userProgress == null)
                return NotFound();

            _context.UserProgresses.Remove(userProgress);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
