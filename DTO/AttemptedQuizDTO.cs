namespace apiTechSkillPro.DTO
{
    public class AttemptedQuizDTO
    {
        public int QuizID { get; set; }
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public int Duration { get; set; }
        public int TotalMarks { get; set; }
        public int AttemptsAllowed { get; set; }
        public DateTime AttemptedAt { get; set; }
        public int AttemptID { get; set; }
    }

}
