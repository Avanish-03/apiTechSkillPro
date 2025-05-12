namespace apiTechSkillPro.DTOs
{
    public class QuizAttemptResponseDTO
    {
        public int AttemptID { get; set; }
        public int UserID { get; set; }
        public int QuizID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Score { get; set; }
        public byte Status { get; set; }
        public int TimeSpent { get; set; }
        public string IPAddress { get; set; }
    }
}
