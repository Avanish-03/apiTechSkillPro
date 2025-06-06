﻿using Microsoft.AspNetCore.Mvc;
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
using apiTechSkillPro.Helpers;

namespace apiTechSkillPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        //private readonly string _jwtSecretKey = "your-very-long-secret-key"; // You should move this to a configuration file

        public AuthController(ApplicationDbContext context, JwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;

        }


        //Register
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
            .Include(u => u.Role) // <-- Add this line to include Role data
            .FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            if (user == null || !VerifyPassword(user.PasswordHash, loginDTO.Password))
            {
                return Unauthorized("Invalid credentials.");
            }

            // Generate JWT Token
            var token = _jwtTokenGenerator.GenerateToken(user.Email, user.RoleID, user.UserID, user.Role.RoleName.ToString());


            // Return token along with user details
            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.UserID,
                    user.Email,
                    user.RoleID,
                    RoleName = user.Role.RoleName.ToString()
                }
            });
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



    }
}
