namespace apiTechSkillPro.DTOs
{
    public class QuestionResponseDTO
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public byte QuestionType { get; set; } // 1=MCQ, 2=True/False, etc.

        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }

        public string CorrectAnswer { get; set; }

        public int Marks { get; set; }
        public byte Difficulty { get; set; } // 1=Easy, 2=Medium, 3=Hard
        public string Explanation { get; set; }

        public int Sequence { get; set; }
    }
}
