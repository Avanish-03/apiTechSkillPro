namespace apiTechSkillPro.DTOs
{
    public class LeaderboardCreateDTO
    {
        public int UserID { get; set; }
        public int QuizID { get; set; }
        public int Score { get; set; }
        public int Rank { get; set; }
    }
}
