using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiTechSkillPro.Data;
using apiTechSkillPro.Models;
using apiTechSkillPro.DTOs;
using System.Text;
using System.Security.Cryptography;

namespace apiTechSkillPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("getprofile")]
        public async Task<IActionResult> GetProfile(int userID)
        {
            if (userID <= 0)
                return BadRequest("Invalid UserID.");

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserID == userID);

            if (user == null)
                return NotFound("User not found.");

            return Ok(new
            {
                userID = user.UserID,
                name = user.FullName,
                email = user.Email,
                roleId = user.RoleID,
                profileImage = user.ProfileImage
            });
        }

        [HttpPut("updateprofile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UserUpdateDTO dto)
        {
            var user = await _context.Users.FindAsync(dto.UserID);
            if (user == null) return NotFound();

            user.FullName = dto.FullName;
            user.Email = dto.Email;

            if (dto.ProfileImage != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ProfileImage.FileName)}";
                var path = Path.Combine("wwwroot/images", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await dto.ProfileImage.CopyToAsync(stream);
                }
                user.ProfileImage = $"/images/{fileName}";
            }

            await _context.SaveChangesAsync();
            return Ok(user);
        }



        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users
                .Include(u => u.Role)
                .ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null)
                return NotFound();

            return user;
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserID)
                return BadRequest();

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.UserID == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Change Password Endpoint
        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (dto == null || dto.UserID <= 0 ||
                string.IsNullOrWhiteSpace(dto.CurrentPassword) ||
                string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return BadRequest("Invalid input data.");
            }

            var user = await _context.Users.FindAsync(dto.UserID);
            if (user == null)
                return NotFound("User not found.");

            string currentHash = HashPassword(dto.CurrentPassword);

            if (user.PasswordHash != currentHash)
                return BadRequest("Current password is incorrect.");

            user.PasswordHash = HashPassword(dto.NewPassword);

            await _context.SaveChangesAsync();

            return Ok("Password changed successfully.");
        }

        // Helper for Hashing
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

    }
}
