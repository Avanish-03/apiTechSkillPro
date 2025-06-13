using System.ComponentModel.DataAnnotations;

namespace apiTechSkillPro.DTOs
{
    public class QuizAttemptCreateDTO
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public int QuizID { get; set; }

        [MaxLength(45)] // Optional, depends on DB schema
        public string IPAddress { get; set; }
    }
}
