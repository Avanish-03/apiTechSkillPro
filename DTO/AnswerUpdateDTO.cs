using System.ComponentModel.DataAnnotations;

namespace apiTechSkillPro.DTOs
{
    public class AnswerUpdateDTO
    {
        public int AnswerID { get; set; }

        [Required]
        public string SelectedOption { get; set; }

        public bool IsCorrect { get; set; }

        public int TimeSpent { get; set; } // in seconds
    }
}
