using System;
using System.ComponentModel.DataAnnotations;
using apiTechSkillPro.Models;

namespace apiTechSkillPro.Models
{
    public class Answer
    {
        [Key]
        public int AnswerID { get; set; }

        public int AttemptID { get; set; }
        public QuizAttempt QuizAttempt { get; set; }

        public int QuestionID { get; set; }
        public Question Question { get; set; }

        public string SelectedOption { get; set; }
        public bool IsCorrect { get; set; }
        public int TimeSpent { get; set; } // in seconds
        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
    }
}