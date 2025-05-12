namespace apiTechSkillPro.DTOs
{
    public class ResultResponseDTO
    {
        public int ResultID { get; set; }
        public int UserID { get; set; }
        public int QuizID { get; set; }
        public int AttemptID { get; set; }
        public int TotalMarks { get; set; }
        public int ObtainedMarks { get; set; }
        public decimal Percentage { get; set; }
        public int? Rank { get; set; }
        public DateTime AttemptDate { get; set; }
        public int TimeTaken { get; set; } // in seconds
    }
}
