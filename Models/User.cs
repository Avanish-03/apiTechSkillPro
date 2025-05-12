using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace apiTechSkillPro.Models
{


        public class User
        {
            [Key]
            public int UserID { get; set; }

            [Required]
            [StringLength(100)]
            public string FullName { get; set; }

            [Required]
            [StringLength(255)]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string PasswordHash { get; set; }

            public int RoleID { get; set; }
            public Role Role { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? LastLogin { get; set; }
            public bool IsActive { get; set; } = true;

            [StringLength(255)]
            public string ProfileImage { get; set; }

            // Navigation properties
            public ICollection<Quiz> CreatedQuizzes { get; set; }
            public ICollection<QuizAttempt> QuizAttempts { get; set; }
            public ICollection<Feedback> Feedbacks { get; set; }
            public ICollection<Notification> Notifications { get; set; }
            public ICollection<UserProgress> UserProgresses { get; set; }
        }
    

}
