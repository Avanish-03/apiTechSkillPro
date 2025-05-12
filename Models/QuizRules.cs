using System.ComponentModel.DataAnnotations;
using apiTechSkillPro.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace apiTechSkillPro.Models
{
    public class QuizRules
    {
        [Key]
        public int RuleID { get; set; }

        public int QuizID { get; set; }
        public Quiz Quiz { get; set; }

        public bool NegativeMarking { get; set; } = false;

        [Column(TypeName = "decimal(3,2)")]
        public decimal NegativeMarkValue { get; set; } = 0.25m;

        public int? TimeLimit { get; set; } // in seconds
        public bool ShowResult { get; set; } = true;
        public bool ShuffleQuestions { get; set; } = true;
    }
}