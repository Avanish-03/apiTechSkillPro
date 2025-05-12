namespace apiTechSkillPro.DTOs
{
    public class UserProgressResponseDTO
    {
        public int ProgressID { get; set; }
        public int UserID { get; set; }
        public int QuizID { get; set; }
        public int LastAttemptID { get; set; }
        public int? BestScore { get; set; }
        public int AttemptsCount { get; set; }
        public byte CompletionStatus { get; set; }
        public string UserFullName { get; set; } // Adding User Full Name
        public string QuizTitle { get; set; } // Adding Quiz Title
    }
}
