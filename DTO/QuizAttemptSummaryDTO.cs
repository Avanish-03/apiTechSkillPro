namespace apiTechSkillPro.DTO
{
    public class QuizAttemptSummaryDTO
    {
        public string StudentName { get; set; }
        public string QuizTitle { get; set; }
        public int Score { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public DateTime AttemptDate { get; set; }
        public string ResultStatus { get; internal set; }
    }
}
