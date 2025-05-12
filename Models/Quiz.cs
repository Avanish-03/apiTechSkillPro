using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using apiTechSkillPro.Models;

namespace apiTechSkillPro.Models
{
    public class Quiz
    {
        [Key]
        public int QuizID { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public int CategoryID { get; set; }
        public Category Category { get; set; }

        public string Description { get; set; }
        public int Duration { get; set; } // in minutes

        public int CreatedBy { get; set; }
        public User Creator { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublished { get; set; } = false;
        public int? PassingScore { get; set; }
        public string Instructions { get; set; }
        public int TotalMarks { get; set; }
        public int AttemptsAllowed { get; set; } = 1;

        // Navigation properties
        public ICollection<Question> Questions { get; set; }
        public ICollection<QuizAttempt> QuizAttempts { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public QuizRules QuizRules { get; set; }
    }
}