using System.ComponentModel.DataAnnotations;

namespace apiTechSkillPro.DTOs
{
    public class QuestionCreateDTO
    {
        [Required]
        public int QuizID { get; set; }

        [Required]
        public string QuestionText { get; set; }

        public byte QuestionType { get; set; } // 1=MCQ, 2=True/False, etc.

        [StringLength(255)]
        public string Option1 { get; set; }

        [StringLength(255)]
        public string Option2 { get; set; }

        [StringLength(255)]
        public string Option3 { get; set; }

        [StringLength(255)]
        public string Option4 { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }

        public int Marks { get; set; } = 1;
        public byte Difficulty { get; set; } // 1=Easy, 2=Medium, 3=Hard
        public string Explanation { get; set; }

        public int Sequence { get; set; }
    }
}
