namespace apiTechSkillPro.DTOs
{
    public class QuizAttemptLogCreateDTO
    {
        public int AttemptID { get; set; }
        public int QuestionID { get; set; }
        public string SelectedOption { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime AttemptTime { get; set; }
    }
}
