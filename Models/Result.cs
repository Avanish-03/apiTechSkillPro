using System;
using System.ComponentModel.DataAnnotations;
using apiTechSkillPro.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace apiTechSkillPro.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public int QuizID { get; set; }
        public Quiz Quiz { get; set; }

        public string? ResultStatus { get; set; } 


        public int AttemptID { get; set; }
        public QuizAttempt QuizAttempt { get; set; }

        public int TotalMarks { get; set; }
        public int ObtainedMarks { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Percentage { get; set; }

        public int? Rank { get; set; }  
        public DateTime AttemptDate { get; set; }
        public int TimeTaken { get; set; } // in seconds
    }
}