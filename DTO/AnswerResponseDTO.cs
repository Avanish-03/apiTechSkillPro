namespace apiTechSkillPro.DTOs
{
    public class AnswerResponseDTO
    {
        public int AnswerID { get; set; }

        public int AttemptID { get; set; }
        public int QuestionID { get; set; }

        public string SelectedOption { get; set; }

        public bool IsCorrect { get; set; }

        public int TimeSpent { get; set; } // in seconds
        public DateTime AnsweredAt { get; set; }
    }
}
