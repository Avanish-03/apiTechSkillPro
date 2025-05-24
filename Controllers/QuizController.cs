using apiTechSkillPro.Data;
using apiTechSkillPro.DTOs;
using apiTechSkillPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace apiTechSkillPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuizController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Quiz
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzes()
        {
            return await _context.Quizzes
                .Include(q => q.Category)
                .Include(q => q.Creator)
                .ToListAsync();
        }

        // GET: api/Quiz/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Quiz>> GetQuiz(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Category)
                .Include(q => q.Creator)
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.QuizID == id);

            if (quiz == null)
                return NotFound();

            return quiz;
        }

        // POST: api/Quiz
        [HttpPost]
        public async Task<ActionResult<Quiz>> PostQuiz([FromForm] QuizCreateDTO dto)
        {
            // TEMP for Swagger Testing
            HttpContext.Session.SetInt32("UserID", 1); // remove this later

            var createdBy = HttpContext.Session.GetInt32("UserID");

            if (!createdBy.HasValue)
            {
                return Unauthorized();
            }

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
                CreatedBy = createdBy.Value
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuiz), new { id = quiz.QuizID }, quiz);
        }


        // PUT: api/Quiz/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuiz(int id, [FromForm] QuizCreateDTO dto)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
                return NotFound();

            quiz.Title = dto.Title;
            quiz.CategoryID = dto.CategoryID;
            quiz.Description = dto.Description;
            quiz.Duration = dto.Duration;
            quiz.Instructions = dto.Instructions;
            quiz.TotalMarks = dto.TotalMarks;
            quiz.AttemptsAllowed = dto.AttemptsAllowed;
            quiz.PassingScore = dto.PassingScore;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Quizzes.Any(q => q.QuizID == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Quiz/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
                return NotFound();

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper method to get current user's ID from JWT token
        private int GetCurrentUserId()
        {
            // Extracting user ID from the JWT token (assumes you are using JWT for authentication)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // JWT token
            return Convert.ToInt32(userId); // Convert to integer
        }
    }
}
