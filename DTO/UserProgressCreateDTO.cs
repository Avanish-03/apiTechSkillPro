namespace apiTechSkillPro.DTOs
{
    public class UserProgressCreateDTO
    {
        public int UserID { get; set; }
        public int QuizID { get; set; }
        public int LastAttemptID { get; set; }
        public int? BestScore { get; set; }
        public int AttemptsCount { get; set; } = 0;
        public byte CompletionStatus { get; set; } // 1=Not Started, 2=In Progress, 3=Completed
    }
}
