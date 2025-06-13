using apiTechSkillPro.Models;
using Microsoft.EntityFrameworkCore;

namespace apiTechSkillPro.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Leaderboard> Leaderboards { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<QuizAttemptLog> QuizAttemptLogs { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<QuizRules> QuizRules { get; set; }
        public DbSet<UserProgress> UserProgresses { get; set; }

        public DbSet<apiTechSkillPro.Models.Leaderboard> Leaderboard { get; set; } = default!;
        public DbSet<apiTechSkillPro.Models.QuizAttemptLog> QuizAttemptLog { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User-Role relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Quiz-Category relationship
            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Category)
                .WithMany(c => c.Quizzes)
                .HasForeignKey(q => q.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Quiz-Creator relationship
            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Creator)
                .WithMany(u => u.CreatedQuizzes)
                .HasForeignKey(q => q.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Question-Quiz relationship
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizID)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Answer relationships
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.QuizAttempt)
                .WithMany(qa => qa.Answers)
                .HasForeignKey(a => a.AttemptID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuizAttemptLog>()
                .HasOne(qal => qal.QuizAttempt1)
                .WithMany(qa => qa.QuizAttemptLogs)
                .HasForeignKey(qal => qal.AttemptID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Result relationships
            modelBuilder.Entity<Result>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Quiz)
                .WithMany()
                .HasForeignKey(r => r.QuizID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.QuizAttempt)
                .WithOne(qa => qa.Result)
                .HasForeignKey<Result>(r => r.AttemptID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Feedback relationships
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Quiz)
                .WithMany(q => q.Feedbacks)
                .HasForeignKey(f => f.QuizID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Notification-User relationship
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure QuizRules-Quiz relationship (one-to-one)
            modelBuilder.Entity<QuizRules>()
                .HasOne(qr => qr.Quiz)
                .WithOne(q => q.QuizRules)
                .HasForeignKey<QuizRules>(qr => qr.QuizID)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure UserProgress relationships
            modelBuilder.Entity<UserProgress>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserProgresses)
                .HasForeignKey(up => up.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserProgress>()
                .HasOne(up => up.Quiz)
                .WithMany()
                .HasForeignKey(up => up.QuizID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProgress>()
                .HasOne(up => up.LastAttempt)
                .WithOne()
                .HasForeignKey<UserProgress>(up => up.LastAttemptID)
                .OnDelete(DeleteBehavior.Restrict);

            // Add unique constraints and indexes for optimization
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<QuizAttempt>()
                .HasIndex(qa => new { qa.UserID, qa.QuizID });

            modelBuilder.Entity<Answer>()
                .HasIndex(a => new { a.AttemptID, a.QuestionID })
                .IsUnique();
        }
    }
}
