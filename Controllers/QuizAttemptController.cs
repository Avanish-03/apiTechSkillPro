using apiTechSkillPro.Data;
using apiTechSkillPro.DTO;
using apiTechSkillPro.DTOs;
using apiTechSkillPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<ActionResult<IEnumerable<QuizAttempt>>> GetAllAttempts()
        {
            var attempts = await _context.QuizAttempts
                .Include(a => a.User)
                .Include(a => a.Quiz)
                .Include(a => a.Answers)
                .ToListAsync();

            return Ok(attempts);
        }


        //AttemptCount/{userID}/{quizID}
        [HttpGet("AttemptCount/{userID}/{quizID}")]
        public IActionResult GetAttemptCount(int userID, int quizID)
        {
            var count = _context.QuizAttempts
                                .Count(q => q.UserID == userID && q.QuizID == quizID);
            return Ok(count);
        }


        // GET: api/QuizAttempt/{attemptId}
        [HttpGet("{attemptId}")]
        public async Task<ActionResult<QuizAttempt>> GetAttemptById(int attemptId)
        {
            var attempt = await _context.QuizAttempts
                .Include(a => a.User)
                .Include(a => a.Quiz)
                .Include(a => a.Answers)
                .FirstOrDefaultAsync(a => a.AttemptID == attemptId);

            return attempt == null ? NotFound("Attempt not found.") : Ok(attempt);
        }

        // ✅ GET Attempt Questions
        [HttpGet("{attemptId}/questions")]
        public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetAttemptQuestions(int attemptId)
        {
            var attempt = await _context.QuizAttempts
                .Include(a => a.Quiz)
                    .ThenInclude(q => q.Questions)
                .FirstOrDefaultAsync(a => a.AttemptID == attemptId);

            if (attempt == null)
                return NotFound("Attempt not found.");

            var questionDTOs = attempt.Quiz.Questions.Select(q => new QuestionDTO
            {
                QuestionID = q.QuestionID,
                QuestionText = q.QuestionText,
                Options = new List<string> { q.Option1, q.Option2, q.Option3, q.Option4 }
            });

            return Ok(questionDTOs);
        }

        // POST: api/QuizAttempt/start
        [HttpPost("start")]
        public async Task<ActionResult> StartQuizAttempt([FromBody] QuizAttemptCreateDTO dto)
        {
            if (dto == null || dto.UserID <= 0 || dto.QuizID <= 0)
                return BadRequest("Invalid UserID or QuizID.");


            var user = await _context.Users.FindAsync(dto.UserID);
            var quiz = await _context.Quizzes.FindAsync(dto.QuizID);

            if (user == null || quiz == null)
                return NotFound("User or Quiz not found.");
            var attempt = new QuizAttempt
            {
                UserID = dto.UserID,
                QuizID = dto.QuizID,
                StartTime = DateTime.UtcNow,
                IPAddress = dto.IPAddress ?? "127.0.0.1",
                Status = 1,
                TimeSpent = 0
            };

            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Quiz attempt started.", attemptID = attempt.AttemptID });
        }

        // GET: api/QuizAttempt/UserQuizzes/{userId}
        [HttpGet("UserQuizzes/{userId}")]
        public async Task<IActionResult> GetUserAttemptedQuizzes(int userId)
        {
            var attemptedQuizzes = await _context.QuizAttempts
                .Include(qa => qa.Quiz)
                    .ThenInclude(q => q.Category)
                .Where(qa => qa.UserID == userId)
                .Select(qa => new AttemptedQuizDTO
                {
                    AttemptID = qa.AttemptID,
                    QuizID = qa.QuizID,
                    Title = qa.Quiz.Title,
                    CategoryName = qa.Quiz.Category.Name,
                    Duration = qa.Quiz.Duration,
                    TotalMarks = qa.Quiz.TotalMarks,
                    AttemptsAllowed = qa.Quiz.AttemptsAllowed,
                    //AttemptedAt = qa.AttemptedAt
                })
                .ToListAsync();

            return Ok(attemptedQuizzes);
        }


        // ✅ POST: Submit Quiz
        [HttpPost("submit")]
        public async Task<ActionResult> SubmitQuizAttempt([FromBody] QuizSubmissionDTO submission)
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
                if (question == null)
                    continue;

                bool isCorrect = string.Equals(
                    question.CorrectAnswer?.Trim(),
                    answerDto.SelectedAnswer?.Trim(),
                    StringComparison.OrdinalIgnoreCase
                );

                if (isCorrect)
                    totalScore += question.Marks;

                answerList.Add(new Answer
                {
                    QuestionID = question.QuestionID,
                    SelectedOption = answerDto.SelectedAnswer,
                    IsCorrect = isCorrect
                });
            }

            var totalMarks = quiz.Questions.Sum(q => q.Marks);
            var percentage = totalMarks == 0 ? 0 : (decimal)totalScore / totalMarks * 100;
            var resultStatus = totalScore >= quiz.PassingScore ? "Pass" : "Fail";

            var attempt = new QuizAttempt
            {
                UserID = submission.UserID,
                QuizID = submission.QuizID,
                StartTime = DateTime.UtcNow.AddSeconds(-submission.TimeTaken),
                EndTime = DateTime.UtcNow,
                TimeSpent = submission.TimeTaken,
                Score = totalScore,
                Status = 2,
                IPAddress = submission.IPAddress ?? "127.0.0.1",
                Answers = answerList
            };

            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            var result = new Result
            {
                UserID = submission.UserID,
                QuizID = submission.QuizID,
                AttemptID = attempt.AttemptID,
                TotalMarks = totalMarks,
                ObtainedMarks = totalScore,
                Percentage = percentage,
                TimeTaken = submission.TimeTaken,
                AttemptDate = DateTime.UtcNow,
                ResultStatus = resultStatus
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            // ✅ Auto-Save to Leaderboard
            var leaderboard = new Leaderboard
            {
                UserID = submission.UserID,
                QuizID = submission.QuizID,
                Score = totalScore,
                Rank = 0 // Placeholder — optionally update later
            };

            _context.Leaderboards.Add(leaderboard);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "✅ Quiz submitted successfully and leaderboard updated.",
                attemptID = attempt.AttemptID,
                resultID = result.ResultID,
                score = totalScore,
                percentage,
                passStatus = resultStatus
            });
        }


        // PUT: api/QuizAttempt/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuizAttempt(int id, [FromBody] QuizAttempt updatedAttempt)
        {
            if (id != updatedAttempt.AttemptID)
                return BadRequest("Mismatched Attempt ID.");

            _context.Entry(updatedAttempt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.QuizAttempts.Any(a => a.AttemptID == id))
                    return NotFound();
                else
                    throw;
            }
        }

        // DELETE: api/QuizAttempt/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuizAttempt(int id)
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
