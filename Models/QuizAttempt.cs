using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using apiTechSkillPro.Models;

namespace apiTechSkillPro.Models
{
    public class QuizAttempt
    {
        [Key]
        public int AttemptID { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public int QuizID { get; set; }
        public Quiz Quiz { get; set; }

        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }
        public int? Score { get; set; }
        public byte Status { get; set; } // 1=In Progress, 2=Completed, etc.
        public int TimeSpent { get; set; } // in seconds
        public string IPAddress { get; set; }

        // Navigation properties
        public ICollection<Answer>? Answers { get; set; }
        public Result? Result { get; set; }
        public UserProgress? UserProgress { get; set; }
        public ICollection<QuizAttemptLog>? QuizAttemptLogs { get; set; }

    }
}