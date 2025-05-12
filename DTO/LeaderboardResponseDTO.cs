namespace apiTechSkillPro.DTOs
{
    public class LeaderboardResponseDTO
    {
        public int LeaderboardID { get; set; }
        public int UserID { get; set; }
        public int QuizID { get; set; }
        public int Score { get; set; }
        public int Rank { get; set; }
        public string UserFullName { get; set; } // Adding User Full Name
        public string QuizTitle { get; set; } // Adding Quiz Title
    }
}
