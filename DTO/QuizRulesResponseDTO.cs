namespace apiTechSkillPro.DTOs
{
    public class QuizRulesResponseDTO
    {
        public int RuleID { get; set; }
        public int QuizID { get; set; }

        public bool NegativeMarking { get; set; } = false;

        public decimal NegativeMarkValue { get; set; } = 0.25m;

        public int? TimeLimit { get; set; } // in seconds

        public bool ShowResult { get; set; } = true;

        public bool ShuffleQuestions { get; set; } = true;
    }
}
