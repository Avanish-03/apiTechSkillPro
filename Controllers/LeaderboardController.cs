using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiTechSkillPro.Data;
using apiTechSkillPro.Models;

namespace apiTechSkillPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeaderboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Leaderboard
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Leaderboard>>> GetLeaderboards()
        {
            return await _context.Leaderboards
                .Include(lb => lb.User)
                .Include(lb => lb.Quiz)
                .ToListAsync();
        }

        // GET: api/Leaderboard/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Leaderboard>> GetLeaderboard(int id)
        {
            var leaderboard = await _context.Leaderboards
                .Include(lb => lb.User)
                .Include(lb => lb.Quiz)
                .FirstOrDefaultAsync(lb => lb.LeaderboardID == id);

            if (leaderboard == null)
                return NotFound();

            return leaderboard;
        }

        // POST: api/Leaderboard
        [HttpPost]
        public async Task<ActionResult<Leaderboard>> PostLeaderboard(Leaderboard leaderboard)
        {
            _context.Leaderboards.Add(leaderboard);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLeaderboard), new { id = leaderboard.LeaderboardID }, leaderboard);
        }

        // PUT: api/Leaderboard/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeaderboard(int id, Leaderboard leaderboard)
        {
            if (id != leaderboard.LeaderboardID)
                return BadRequest();

            _context.Entry(leaderboard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaderboardExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Leaderboard/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaderboard(int id)
        {
            var leaderboard = await _context.Leaderboards.FindAsync(id);
            if (leaderboard == null)
                return NotFound();

            _context.Leaderboards.Remove(leaderboard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeaderboardExists(int id)
        {
            return _context.Leaderboards.Any(e => e.LeaderboardID == id);
        }
    }
}
