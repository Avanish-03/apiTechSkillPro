using apiTechSkillPro.Data;
using apiTechSkillPro.DTOs;
using apiTechSkillPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace apiTechSkillPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "1")] // Admin only
    public class QuizController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuizController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Quiz
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetQuizzes()
        {
            var quizzes = await _context.Quizzes
                .Include(q => q.Category)
                .Include(q => q.Creator)
                .Include(q => q.Questions)
                .Include(q => q.QuizAttempts)
                .Select(q => new
                {
                    q.QuizID,
                    q.Title,
                    q.Description,
                    q.Duration,
                    q.TotalMarks,
                    q.AttemptsAllowed,
                    q.PassingScore,
                    q.Instructions,
                    q.IsPublished,
                    q.CreatedAt,
                    CategoryName = q.Category.Name,
                    CreatedBy = q.Creator.FullName,
                    CreatedByEmail = q.Creator.Email,
                    CreatedAtFormatted = q.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    NumberOfQuestions = q.Questions.Count,
                    NumberOfAttempts = q.QuizAttempts.Count
                })
                .ToListAsync();

            if (quizzes == null || quizzes.Count == 0)
            {
                return NoContent();
            }

            return Ok(quizzes);
        }

        // GET: api/Quiz/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetQuiz(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Category)
                .Include(q => q.Creator)
                .Include(q => q.Questions)
                .Include(q => q.QuizAttempts)
                .ThenInclude(qa => qa.User)
                .Select(q => new
                {
                    q.QuizID,
                    q.Title,
                    q.Description,
                    q.Duration,
                    q.TotalMarks,
                    q.AttemptsAllowed,
                    q.PassingScore,
                    q.Instructions,
                    q.IsPublished,
                    q.CreatedAt,
                    CategoryName = q.Category.Name,
                    CreatedBy = q.Creator.FullName,
                    CreatedByEmail = q.Creator.Email,
                    CreatedAtFormatted = q.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    QuestionCount = q.Questions.Count,
                    AttemptDetails = q.QuizAttempts.Select(qa => new
                    {
                        qa.User.FullName,
                        qa.User.Email,
                        qa.Score,
                        qa.StartTime,
                        qa.EndTime,
                        qa.Result
                    }).ToList()
                })
                .FirstOrDefaultAsync(q => q.QuizID == id);

            if (quiz == null)
            {
                return NotFound();
            }

            return Ok(quiz);
        }

        // POST: api/Quiz
        [HttpPost]
        public async Task<ActionResult> CreateQuiz([FromForm] QuizCreateDTO dto)
        {
            var quiz = new Quiz
            {
                Title = dto.Title,
                CategoryID = dto.CategoryID,
                Description = dto.Description,
                Duration = dto.Duration,
                Instructions = dto.Instructions,
                TotalMarks = dto.TotalMarks,
                AttemptsAllowed = dto.AttemptsAllowed,
                PassingScore = dto.PassingScore,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy,
                IsPublished = dto.IsPublished // ✅ Added
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Quiz created successfully", quiz.QuizID });
        }

        // PUT: api/Quiz/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuiz(int id, [FromForm] QuizCreateDTO dto)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            quiz.Title = dto.Title;
            quiz.CategoryID = dto.CategoryID;
            quiz.Description = dto.Description;
            quiz.Duration = dto.Duration;
            quiz.Instructions = dto.Instructions;
            quiz.TotalMarks = dto.TotalMarks;
            quiz.AttemptsAllowed = dto.AttemptsAllowed;
            quiz.PassingScore = dto.PassingScore;
            quiz.IsPublished = dto.IsPublished; // ✅ Added

            await _context.SaveChangesAsync();
            return Ok(new { message = "Quiz updated successfully" });
        }

        // PATCH: api/Quiz/publish/5?publish=true
        [HttpPatch("publish/{id}")]
        public async Task<IActionResult> TogglePublish(int id, [FromQuery] bool publish)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            quiz.IsPublished = publish;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Quiz {(publish ? "published" : "unpublished")} successfully." });
        }

        // DELETE: api/Quiz/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Quiz deleted successfully" });
        }

        private int GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Convert.ToInt32(userId);
        }
    }
}
