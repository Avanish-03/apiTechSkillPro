namespace apiTechSkillPro.DTOs
{
    public class FeedbackCreateDTO
    {
        public int UserID { get; set; }
        public int QuizID { get; set; }
        public byte Rating { get; set; }
        public string Comments { get; set; }
        public bool IsAnonymous { get; set; } = false;
    }
}
