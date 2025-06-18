using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiTechSkillPro.Data;
using apiTechSkillPro.Models;
using apiTechSkillPro.DTOs;

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

        // ✅ Get all results
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Result>>> GetResults()
        {
            return await _context.Results
                .Include(r => r.User)
                .Include(r => r.Quiz)
                .Include(r => r.QuizAttempt)
                .ToListAsync();
        }

        // ✅ Get result by ResultID
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

        // ✅ Get results by user
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Result>>> GetResultsByUser(int userId)
        {
            return await _context.Results
                .Include(r => r.Quiz)
                .Where(r => r.UserID == userId)
                .ToListAsync();
        }

        // ✅ Get results by quiz
        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<IEnumerable<Result>>> GetResultsByQuiz(int quizId)
        {
            return await _context.Results
                .Include(r => r.User)
                .Where(r => r.QuizID == quizId)
                .ToListAsync();
        }

        // ✅ Post result using DTO
        [HttpPost]
        public async Task<ActionResult<Result>> PostResult(ResultCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = new Result
            {
                UserID = dto.UserID,
                QuizID = dto.QuizID,
                AttemptID = dto.AttemptID,
                TotalMarks = dto.TotalMarks,
                ObtainedMarks = dto.ObtainedMarks,
                Percentage = dto.Percentage,
                Rank = dto.Rank,
                TimeTaken = dto.TimeTaken,
                ResultStatus = dto.ResultStatus ?? "Pending"

            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetResult), new { id = result.ResultID }, result);
        }

        // ✅ Update result
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

        // ✅ Get result by UserID and QuizID
        [HttpGet("user/{userId}/quiz/{quizId}")]
        public async Task<ActionResult<Result>> GetResultByUserAndQuiz(int userId, int quizId)
        {
            var result = await _context.Results
                .Include(r => r.Quiz)
                .Include(r => r.QuizAttempt)
                .FirstOrDefaultAsync(r => r.UserID == userId && r.QuizID == quizId);

            if (result == null)
                return NotFound("Result not found for this quiz and user.");

            return Ok(result);
        }

        // ✅ Get student results for quizzes created by this teacher (using UserID)
        [HttpGet("createdby/{userId}/students-results")]
        public async Task<ActionResult<IEnumerable<object>>> GetResultsByTeacherUserID(int userId)
        {
            // Ensure this user is actually a teacher
            var isTeacher = await _context.Users.AnyAsync(u => u.UserID == userId && u.RoleID == 3);
            if (!isTeacher)
                return BadRequest("User is not a teacher.");

            // Get results where the quiz is created by this teacher
            var results = await _context.Results
                .Include(r => r.User)
                .Include(r => r.Quiz)
                .Where(r => r.Quiz.CreatedBy == userId) // <<== Quiz CreatedBy = Teacher's UserID
                .Select(r => new
                {
                    StudentName = r.User.FullName,
                    QuizTitle = r.Quiz.Title,
                    r.ObtainedMarks,
                    r.TotalMarks,
                    r.Percentage,
                    r.Rank,
                    r.ResultStatus,
                    r.TimeTaken
                })
                .ToListAsync();

            if (results == null || results.Count == 0)
                return NotFound("No results found for quizzes created by this teacher.");

            return Ok(results);
        }



        // ✅ Delete result
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
