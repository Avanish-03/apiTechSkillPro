using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiTechSkillPro.Models;
using apiTechSkillPro.Data;
using apiTechSkillPro.DTO;
using apiTechSkillPro.DTOs;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Humanizer;

namespace apiTechSkillPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _jwtSecretKey = "your-very-long-secret-key"; // You should move this to a configuration file

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }


        //Register
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromForm] UserRegisterDTO signupDTO)
        {
            if (signupDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            var existingUser = await _context.Users
                                             .FirstOrDefaultAsync(u => u.Email == signupDTO.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already in use.");
            }

            var role = await _context.Roles
                                     .FirstOrDefaultAsync(r => r.RoleID == signupDTO.RoleID);
            if (role == null)
            {
                return BadRequest($"Invalid role ID: {signupDTO.RoleID}. Please provide a valid RoleID.");
            }

            var user = new User
            {
                FullName = signupDTO.FullName,
                Email = signupDTO.Email,
                PasswordHash = HashPassword(signupDTO.Password),
                RoleID = signupDTO.RoleID
            };

            if (signupDTO.ProfileImage != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(signupDTO.ProfileImage.FileName);
                var filePath = Path.Combine("wwwroot/profileimages", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await signupDTO.ProfileImage.CopyToAsync(stream);
                }

                user.ProfileImage = "/profileimages/" + fileName;
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }


        // Login endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDTO)
        {
            if (loginDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            // Find the user by email
            var user = await _context.Users
                                      .FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            if (user == null || !VerifyPassword(user.PasswordHash, loginDTO.Password))
            {
                return Unauthorized("Invalid credentials.");
            }

            // Generate JWT Token
            var token = GenerateJwtToken(user);

            // Set user ID into session
            HttpContext.Session.SetInt32("UserID", user.UserID);

            // Return token along with user details
            return Ok(new { token, userID = user.UserID, fullName = user.FullName, email = user.Email, roleId = user.RoleID });
        }




        // Helper method for password hashing
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }

        // Helper method for verifying password hash
        private bool VerifyPassword(string storedHash, string inputPassword)
        {
            var inputHash = HashPassword(inputPassword);
            return storedHash == inputHash;
        }

        // Helper method to generate JWT token
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("YourSecretKeyHereWhichIsAtLeast32CharactersLong123!"));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var token = new JwtSecurityToken(
                issuer: "TechSkillPro",
                audience: "Users",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
