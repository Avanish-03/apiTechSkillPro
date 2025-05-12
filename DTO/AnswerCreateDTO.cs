using System.ComponentModel.DataAnnotations;

namespace apiTechSkillPro.DTOs
{
    public class AnswerCreateDTO
    {
        public int AttemptID { get; set; }

        public int QuestionID { get; set; }

        [Required]
        public string SelectedOption { get; set; }

        public bool IsCorrect { get; set; }

        public int TimeSpent { get; set; } // in seconds
    }
}
