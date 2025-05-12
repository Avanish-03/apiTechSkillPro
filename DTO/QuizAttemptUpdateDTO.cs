namespace apiTechSkillPro.DTOs
{
    public class QuizAttemptUpdateDTO
    {
        public int AttemptID { get; set; }
        public int? Score { get; set; }
        public byte Status { get; set; }
        public int TimeSpent { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
