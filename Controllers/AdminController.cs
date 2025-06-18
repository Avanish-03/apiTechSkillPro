using System.Security.Cryptography;
using System.Text;
using apiTechSkillPro.Data;
using apiTechSkillPro.DTO;
using apiTechSkillPro.DTOs;
using apiTechSkillPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiTechSkillPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }


        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalUsers = await _context.Users.CountAsync(u => u.RoleID != 1); // Students
            var totalQuizzes = await _context.Quizzes.CountAsync();
            var totalFeedbacks = await _context.ContactMessages.CountAsync();
            var totalResults = await _context.Results.CountAsync();

            var stats = new
            {
                TotalUsers = totalUsers,
                TotalQuizzes = totalQuizzes,
                Feedbacks = totalFeedbacks,
                Results = totalResults
            };

            return Ok(new
            {
                totalUsers = totalUsers,
                totalQuizzes = totalQuizzes,
                feedbacks = totalFeedbacks,
                results = totalResults
            });
        }

        //GetAllUser
        [HttpGet("getallusers")]
        //[Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Where(u => u.RoleID == 2 || u.RoleID == 3) // Only Students & Teachers
                .Select(u => new
                {
                    u.UserID,
                    u.FullName,
                    u.Email,
                    u.RoleID,
                    u.ProfileImage,
                    Role = _context.Roles
                        .Where(r => r.RoleID == u.RoleID)
                        .Select(r => r.RoleName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(users);
        }


        //Search
        [HttpGet("searchusers")]
        public async Task<IActionResult> SearchUsers([FromQuery] string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest(new { message = "Search keyword is required." });
            }

            var users = await _context.Users
                .Where(u => (u.RoleID == 2 || u.RoleID == 3) &&
                       (u.FullName.ToLower().Contains(keyword.ToLower()) ||
                        u.Email.ToLower().Contains(keyword.ToLower())))
                .Select(u => new
                {
                    u.UserID,
                    u.FullName,
                    u.Email,
                    u.RoleID,
                    u.ProfileImage,
                    Role = _context.Roles
                        .Where(r => r.RoleID == u.RoleID)
                        .Select(r => r.RoleName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/Admin/quiz-attempts
        [HttpGet("quiz-attempts")]
        public async Task<ActionResult<IEnumerable<QuizAttemptSummaryDTO>>> GetQuizAttemptsSummary()
        {
            var attempts = await _context.QuizAttempts
                .Include(a => a.User)
                .Include(a => a.Quiz)
                .Select(a => new QuizAttemptSummaryDTO
                {
                    StudentName = a.User.FullName,
                    QuizTitle = a.Quiz.Title,
                    Score = a.Score ?? 0,
                    TotalMarks = a.Quiz.Questions.Sum(q => q.Marks),
                    PassingMarks = a.Quiz.PassingScore ?? 0,
                    ResultStatus = (a.Score ?? 0) >= (a.Quiz.PassingScore ?? 0) ? "Pass" : "Fail",

                })
                .ToListAsync();

            return Ok(attempts);
        }



        // Add a new user
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromForm] UserRegisterDTO signupDTO)
        {
            if (signupDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == signupDTO.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already in use.");
            }

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleID == signupDTO.RoleID);
            if (role == null || (signupDTO.RoleID != 2 && signupDTO.RoleID != 3))
            {
                return BadRequest("Invalid role. Only Student or Teacher roles are allowed.");
            }

            string imagePath = null;

            if (signupDTO.ProfileImage != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(signupDTO.ProfileImage.FileName);
                var savePath = Path.Combine("wwwroot/profileimages", fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await signupDTO.ProfileImage.CopyToAsync(stream);
                }

                imagePath = "/profileimages/" + fileName;
            }

            var user = new User
            {
                FullName = signupDTO.FullName,
                Email = signupDTO.Email,
                PasswordHash = HashPassword(signupDTO.Password),
                RoleID = signupDTO.RoleID,
                ProfileImage = imagePath
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }


        // Update User Details
        [HttpPut("updateuser")]
        public async Task<IActionResult> UpdateUser([FromForm] UserUpdateDTO dto)
        {
            var user = await _context.Users.FindAsync(dto.UserID);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.IsActive = dto.IsActive;

            if (dto.ProfileImage != null)
            {
                // Optional: delete old image if needed

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ProfileImage.FileName);
                var filePath = Path.Combine("wwwroot/profileimages", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ProfileImage.CopyToAsync(stream);
                }

                user.ProfileImage = "/profileimages/" + fileName;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User updated successfully" });
        }


        //Delete User Details
        [HttpDelete("deleteuser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserID == id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok(new { message = "User deleted successfully" });
        }


    }

}
