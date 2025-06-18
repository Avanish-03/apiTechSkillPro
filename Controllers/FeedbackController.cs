using apiTechSkillPro.Data;
using apiTechSkillPro.DTOs;
using apiTechSkillPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiTechSkillPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Feedback
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacks()
        {
            return await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Quiz)
                .ToListAsync();
        }

        // GET: api/Feedback/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Feedback>> GetFeedback(int id)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Quiz)
                .FirstOrDefaultAsync(f => f.FeedbackID == id);

            if (feedback == null)
                return NotFound();

            return feedback;
        }

        // GET: api/Feedback/ByTeacher/5
        [HttpGet("ByTeacher/{teacherId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetFeedbackByTeacher(int teacherId)
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Quiz)
                .Include(f => f.User)
                .Where(f => f.Quiz.CreatedBy == teacherId)
                .Select(f => new
                {
                    f.FeedbackID,
                    f.Rating,
                    f.Comments,
                    f.IsAnonymous,
                    f.SubmittedAt,
                    QuizTitle = f.Quiz.Title,
                    StudentName = f.IsAnonymous ? "Anonymous" : f.User.FullName
                })
                .ToListAsync();

            if (feedbacks == null || feedbacks.Count == 0)
                return NotFound("No feedback found for quizzes created by this teacher.");

            return Ok(feedbacks);
        }


        // POST: api/Feedback
        [HttpPost]
        public async Task<ActionResult<Feedback>> PostFeedback(FeedbackCreateDTO dto)
        {
            // Validate Rating
            if (dto.Rating < 1 || dto.Rating > 5)
                return BadRequest("Rating must be between 1 and 5.");

            // Check User and Quiz existence
            var userExists = await _context.Users.AnyAsync(u => u.UserID == dto.UserID);
            var quizExists = await _context.Quizzes.AnyAsync(q => q.QuizID == dto.QuizID);

            if (!userExists || !quizExists)
                return BadRequest("Invalid UserID or QuizID.");

            // Map DTO to Entity
            var feedback = new Feedback
            {
                UserID = dto.UserID,
                QuizID = dto.QuizID,
                Rating = dto.Rating,
                Comments = dto.Comments,
                IsAnonymous = dto.IsAnonymous,
                SubmittedAt = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFeedback), new { id = feedback.FeedbackID }, feedback);
        }

        // PUT: api/Feedback/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeedback(int id, Feedback feedback)
        {
            if (id != feedback.FeedbackID)
                return BadRequest("Feedback ID mismatch.");

            _context.Entry(feedback).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Feedback/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
                return NotFound();

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedbacks.Any(f => f.FeedbackID == id);
        }
    }
}
