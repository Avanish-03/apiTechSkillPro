using apiTechSkillPro.Data;
using apiTechSkillPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiTechSkillPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalUsers = await _context.Users.CountAsync(u => u.RoleID == 2); // Students
            var totalQuizzes = await _context.Quizzes.CountAsync();
            var totalFeedbacks = await _context.Feedbacks.CountAsync();
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

    }

}
