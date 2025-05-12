using System;
using System.ComponentModel.DataAnnotations;
using apiTechSkillPro.Models;

namespace apiTechSkillPro.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public int QuizID { get; set; }
        public Quiz Quiz { get; set; }

        public byte Rating { get; set; } // 1-5
        public string Comments { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public bool IsAnonymous { get; set; } = false;
    }
}