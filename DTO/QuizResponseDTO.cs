namespace apiTechSkillPro.DTOs
{
    public class QuizResponseDTO
    {
        public int QuizID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; } // in minutes
        public int? PassingScore { get; set; }
        public string Instructions { get; set; }
        public int TotalMarks { get; set; }
        public bool IsPublished { get; set; }
        public int AttemptsAllowed { get; set; }

        // You can also include the category name or full category info
        public string CategoryName { get; set; }

        // Optionally, add other related objects, such as QuizRules, Questions, etc.
        public QuizRulesResponseDTO QuizRules { get; set; }
    }
}
