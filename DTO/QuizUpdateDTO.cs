using System.ComponentModel.DataAnnotations;

namespace apiTechSkillPro.DTOs
{
    public class QuizUpdateDTO
    {
        public int QuizID { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public int CategoryID { get; set; }

        public string Description { get; set; }

        public int Duration { get; set; } // in minutes

        public string Instructions { get; set; }

        public int TotalMarks { get; set; }

        public int AttemptsAllowed { get; set; } = 1;

        public int? PassingScore { get; set; }
    }
}
