using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apiTechSkillPro.Models
{
    public class QuizAttemptLog
    {
        [Key]
        public int LogID { get; set; }
        //[ForeignKey("AttemptID")]
        public int AttemptID { get; set; }
        public int QuestionID { get; set; }
        public string SelectedOption { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime AttemptTime { get; set; }

        // Navigation Properties
        public QuizAttempt? QuizAttempt1 { get; set; }
        public Question Question { get; set; }
    }
}
