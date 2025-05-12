using System.ComponentModel.DataAnnotations;

namespace apiTechSkillPro.Models
{
    public class Leaderboard
    {
        [Key]
        public int LeaderboardID { get; set; }
        public int UserID { get; set; }
        public int QuizID { get; set; }
        public int Score { get; set; }
        public int Rank { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Quiz Quiz { get; set; }
    }

}
