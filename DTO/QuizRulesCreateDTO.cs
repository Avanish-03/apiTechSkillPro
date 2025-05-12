using System.ComponentModel.DataAnnotations;

namespace apiTechSkillPro.DTOs
{
    public class QuizRulesCreateDTO
    {
        public int QuizID { get; set; }

        public bool NegativeMarking { get; set; } = false;

        [Range(0, 1)]
        public decimal NegativeMarkValue { get; set; } = 0.25m;

        public int? TimeLimit { get; set; } // in seconds

        public bool ShowResult { get; set; } = true;

        public bool ShuffleQuestions { get; set; } = true;
    }
}
