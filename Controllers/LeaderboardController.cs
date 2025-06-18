using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiTechSkillPro.Data;
using apiTechSkillPro.Models;
using apiTechSkillPro.DTOs;

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

        //[HttpGet("user/{userId}/quiz/{quizId}")]
        [HttpGet("user/{userId}/quiz/{quizId}")]
        public async Task<ActionResult<Leaderboard>> GetLeaderboardByUserAndQuiz(int userId, int quizId)
        {
            var leaderboard = await _context.Leaderboards
                .Include(lb => lb.User)
                .Include(lb => lb.Quiz)
                .FirstOrDefaultAsync(lb => lb.UserID == userId && lb.QuizID == quizId);

            if (leaderboard == null)
                return NotFound();

            return Ok(leaderboard);
        }


        // GET: api/Leaderboard/search?userId=1&quizId=2
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Leaderboard>>> SearchLeaderboard([FromQuery] int? userId, [FromQuery] int? quizId)
        {
            var query = _context.Leaderboards
                .Include(lb => lb.User)
                .Include(lb => lb.Quiz)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(lb => lb.UserID == userId.Value);

            if (quizId.HasValue)
                query = query.Where(lb => lb.QuizID == quizId.Value);

            var result = await query.ToListAsync();
            return result;
        }

        // POST: api/Leaderboard
        [HttpPost]
        public async Task<ActionResult<Leaderboard>> PostLeaderboard(LeaderboardCreateDTO dto)
        {
            // Check for duplicate (same UserID & QuizID)
            var exists = await _context.Leaderboards
                .AnyAsync(lb => lb.UserID == dto.UserID && lb.QuizID == dto.QuizID);

            if (exists)
                return BadRequest("Leaderboard entry already exists for this user and quiz.");

            var leaderboard = new Leaderboard
            {
                UserID = dto.UserID,
                QuizID = dto.QuizID,
                Score = dto.Score,
                Rank = dto.Rank
            };

            _context.Leaderboards.Add(leaderboard);
            await _context.SaveChangesAsync();

            // Load navigation props
            await _context.Entry(leaderboard).Reference(lb => lb.User).LoadAsync();
            await _context.Entry(leaderboard).Reference(lb => lb.Quiz).LoadAsync();

            return CreatedAtAction(nameof(GetLeaderboard), new { id = leaderboard.LeaderboardID }, leaderboard);
        }

        // GET: api/Leaderboard/createdby/{teacherId}/students-results
        
        [HttpGet("createdby/{teacherId}/students-results")]
        public async Task<ActionResult<IEnumerable<object>>> GetResultsByTeacher(int teacherId)
        {
            var results = await _context.Leaderboards
                .Include(lb => lb.User)
                .Include(lb => lb.Quiz)
                .Where(lb => lb.Quiz.CreatedBy == teacherId)
                .OrderByDescending(lb => lb.Score)
                .Select(lb => new
                {
                    studentName = lb.User.FullName,
                    quizTitle = lb.Quiz.Title,
                    score = lb.Score,
                    rank = lb.Rank
                })
                .ToListAsync();

            return Ok(results);
        }



        // PUT: api/Leaderboard/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeaderboard(int id, LeaderboardCreateDTO dto)
        {
            var leaderboard = await _context.Leaderboards.FindAsync(id);
            if (leaderboard == null)
                return NotFound();

            leaderboard.UserID = dto.UserID;
            leaderboard.QuizID = dto.QuizID;
            leaderboard.Score = dto.Score;
            leaderboard.Rank = dto.Rank;

            await _context.SaveChangesAsync();
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
    }
}
