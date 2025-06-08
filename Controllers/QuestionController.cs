using apiTechSkillPro.Data;
using apiTechSkillPro.DTOs;
using apiTechSkillPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTechSkillPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ GET all questions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestions()
        {
            var questions = await _context.Questions.ToListAsync();  // Removed .Include(q => q.Quiz) to avoid circular references

            return Ok(questions);
        }

        // ✅ GET single question
        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestion(int id)
        {
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.QuestionID == id);

            if (question == null)
                return NotFound(new { message = "Question not found." });

            return Ok(question);
        }

        // ✅ POST create question using DTO
        [HttpPost]
        public async Task<ActionResult> PostQuestion([FromBody] QuestionCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var quiz = await _context.Quizzes.FindAsync(dto.QuizID);
            if (quiz == null)
                return BadRequest(new { message = "QuizID not valid." });

            var question = new Question
            {
                QuizID = dto.QuizID,
                QuestionText = dto.QuestionText,
                QuestionType = dto.QuestionType,
                Option1 = dto.Option1,
                Option2 = dto.Option2,
                Option3 = dto.Option3,
                Option4 = dto.Option4,
                CorrectAnswer = dto.CorrectAnswer,
                Marks = dto.Marks,
                Difficulty = dto.Difficulty,
                Explanation = dto.Explanation,
                Sequence = dto.Sequence
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Question added successfully.",
                questionID = question.QuestionID
            });
        }

        // ✅ PUT update question
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestion(int id, [FromBody] QuestionCreateDTO dto)
        {
            var existingQuestion = await _context.Questions.FindAsync(id);
            if (existingQuestion == null)
                return NotFound(new { message = "Question not found." });

            existingQuestion.QuestionText = dto.QuestionText;
            existingQuestion.QuestionType = dto.QuestionType;
            existingQuestion.Option1 = dto.Option1;
            existingQuestion.Option2 = dto.Option2;
            existingQuestion.Option3 = dto.Option3;
            existingQuestion.Option4 = dto.Option4;
            existingQuestion.CorrectAnswer = dto.CorrectAnswer;
            existingQuestion.Marks = dto.Marks;
            existingQuestion.Difficulty = dto.Difficulty;
            existingQuestion.Explanation = dto.Explanation;
            existingQuestion.Sequence = dto.Sequence;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Question updated successfully." });
        }

        // ✅ DELETE question
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
                return NotFound(new { message = "Question not found." });

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Question deleted successfully." });
        }

        // ✅ GET all questions for a specific quiz
        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestionsByQuizId(int quizId)
        {
            var questions = await _context.Questions
                .Where(q => q.QuizID == quizId)
                .OrderBy(q => q.Sequence)  // Ensure questions are ordered based on sequence
                .ToListAsync();

            // Instead of null, return empty list if no questions are found
            if (!questions.Any())
                return NotFound(new { message = "No questions found for this quiz." });

            return Ok(questions);
        }
    }
}
