using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System;
using apiTechSkillPro.Data;
using apiTechSkillPro.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace apiTechSkillPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContactFormDTO model)
        {
            try
            {
                var contact = new ContactMessage
                {
                    Name = model.Name,
                    Email = model.Email,
                    Message = model.Message,
                    SentAt = DateTime.UtcNow
                };

                _context.ContactMessages.Add(contact);
                await _context.SaveChangesAsync();

                var fromAddress = new MailAddress("vash6365@gmail.com", "Quiz System");
                var toAddress = new MailAddress("vash6365@gmail.com", "Admin");
                const string fromPassword = "bqkg trhl zjxw rgnb";

                string subject = $"📩 New Message from {model.Name} ({model.Email})";
                string body = $"👤 Name: {model.Name}\n📧 Email: {model.Email}\n\n📝 Message:\n{model.Message}";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    Timeout = 20000,
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                })
                {
                    message.ReplyToList.Add(new MailAddress(model.Email, model.Name));
                    smtp.Send(message);
                }

                return Ok("✅ Email sent and message saved successfully.");
            }
            catch (DbUpdateException dbEx)
            {
                return BadRequest("❌ DB Update Error: " + (dbEx.InnerException?.Message ?? dbEx.Message));
            }
            catch (Exception ex)
            {
                return BadRequest("❌ General Error: " + ex.Message);
            }
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages()
        {
            var messages = await _context.ContactMessages
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
            return Ok(messages);
        }
    }

    public class ContactFormDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}
