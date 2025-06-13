using apiTechSkillPro.Data;
using apiTechSkillPro.Models;
using apiTechSkillPro.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using apiTechSkillPro.DTO;

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
                .Include(a => a.Answers)
                .ToListAsync();
        }

        // GET: api/QuizAttempt/{attemptID}/questions
        [HttpGet("{attemptID}/questions")]
        public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetAttemptQuestions(int attemptID)
        {
            var attempt = await _context.QuizAttempts
                .Include(a => a.Quiz)
                    .ThenInclude(q => q.Questions)
                .FirstOrDefaultAsync(a => a.AttemptID == attemptID);

            if (attempt == null)
                return NotFound("Attempt not found.");

            var questionDTOs = attempt.Quiz.Questions.Select(q => new QuestionDTO
            {
                QuestionID = q.QuestionID,
                QuestionText = q.QuestionText,
                Options = new List<string> { q.Option1, q.Option2, q.Option3, q.Option4 }
            }).ToList();

            return Ok(questionDTOs);
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

        // POST: api/QuizAttempt/start
        [HttpPost("start")]
        public async Task<ActionResult> StartAttempt([FromBody] QuizAttemptCreateDTO dto)
        {
            // Basic validation
            if (dto == null)
                return BadRequest("Request body is null.");
            if (dto.UserID <= 0 || dto.QuizID <= 0)
                return BadRequest("Invalid UserID or QuizID.");

            // Check FK existence
            var user = await _context.Users.FindAsync(dto.UserID);
            var quiz = await _context.Quizzes.FindAsync(dto.QuizID);
            if (user == null || quiz == null)
                return BadRequest("User or Quiz not found.");

            try
            {
                var attempt = new QuizAttempt
                {
                    UserID = dto.UserID,
                    QuizID = dto.QuizID,
                    StartTime = DateTime.UtcNow,
                    IPAddress = dto.IPAddress ?? "127.0.0.1",
                    Status = 1,     // 1 = In Progress
                    TimeSpent = 0      // at start, 0 seconds
                };

                _context.QuizAttempts.Add(attempt);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Quiz attempt started.",
                    attemptID = attempt.AttemptID
                });
            }
            catch (Exception ex)
            {
                // return the real error for debugging
                return StatusCode(500, new { error = ex.Message });
            }
        }

        //allquiz attempt by user
        [HttpGet("UserQuizzes/{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAttemptedQuizzes(int userId)
        {
            var attemptedQuizzes = await _context.QuizAttempts
                .Where(q => q.UserID == userId)
                .Include(q => q.Quiz)
                .Select(q => new {
                    q.QuizID,
                    q.Quiz.Title
                })
                .Distinct()
                .ToListAsync();

            return Ok(attemptedQuizzes);
        }


        // POST: api/QuizAttempt/submit
        [HttpPost("submit")]
        public async Task<ActionResult> SubmitAttempt([FromBody] QuizSubmissionDTO submission)
        {
            if (submission == null || submission.Answers == null || !submission.Answers.Any())
                return BadRequest("Invalid submission.");

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.QuizID == submission.QuizID);

            if (quiz == null)
                return NotFound("Quiz not found.");

            int totalScore = 0;
            List<Answer> answerList = new();

            foreach (var answerDto in submission.Answers)
            {
                var question = quiz.Questions.FirstOrDefault(q => q.QuestionID == answerDto.QuestionID);
                if (question == null) continue;

                bool isCorrect = question.CorrectAnswer.Trim().Equals(answerDto.SelectedAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
                if (isCorrect)
                    totalScore += question.Marks;

                answerList.Add(new Answer
                {
                    QuestionID = question.QuestionID,
                    SelectedOption = answerDto.SelectedAnswer,
                    IsCorrect = isCorrect
                });
            }

            string resultStatus = totalScore >= quiz.PassingScore ? "Pass" : "Fail";

            var attempt = new QuizAttempt
            {
                UserID = submission.UserID,
                QuizID = submission.QuizID,
                StartTime = DateTime.UtcNow.AddMinutes(-submission.TimeTaken), // approximate
                EndTime = DateTime.UtcNow,
                TimeSpent = submission.TimeTaken,
                Score = totalScore,
                Status = 2, // Completed
                IPAddress = submission.IPAddress,
                Answers = answerList
            };

            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync(); // to get AttemptID for foreign key

            var result = new Result
            {
                UserID = submission.UserID,
                QuizID = submission.QuizID,
                AttemptID = attempt.AttemptID,
                TotalMarks = quiz.Questions.Sum(q => q.Marks),
                ObtainedMarks = totalScore,
                Percentage = (decimal)totalScore / quiz.Questions.Sum(q => q.Marks) * 100,
                TimeTaken = submission.TimeTaken,
                AttemptDate = DateTime.UtcNow
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Quiz submitted successfully.",
                attemptID = attempt.AttemptID,
                resultID = result.ResultID,
                score = totalScore,
                percentage = result.Percentage,
                passStatus = resultStatus
            });
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
