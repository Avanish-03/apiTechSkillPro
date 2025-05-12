using System.ComponentModel.DataAnnotations;
using apiTechSkillPro.Models;

namespace apiTechSkillPro.Models
{
    public class UserProgress
    {
        [Key]
        public int ProgressID { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public int QuizID { get; set; }
        public Quiz Quiz { get; set; }

        public int LastAttemptID { get; set; }
        public QuizAttempt LastAttempt { get; set; }

        public int? BestScore { get; set; }
        public int AttemptsCount { get; set; } = 0;
        public byte CompletionStatus { get; set; } // 1=Not Started, 2=In Progress, 3=Completed
    }
}