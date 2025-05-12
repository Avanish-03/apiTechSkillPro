namespace apiTechSkillPro.DTOs
{
    public class FeedbackResponseDTO
    {
        public int FeedbackID { get; set; }
        public int UserID { get; set; }
        public string UserFullName { get; set; } // Adding User Full Name
        public int QuizID { get; set; }
        public string QuizTitle { get; set; } // Adding Quiz Title
        public byte Rating { get; set; }
        public string Comments { get; set; }
        public DateTime SubmittedAt { get; set; }
        public bool IsAnonymous { get; set; }
    }
}
