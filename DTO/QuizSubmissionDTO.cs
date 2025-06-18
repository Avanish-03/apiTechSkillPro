namespace apiTechSkillPro.DTO
{
    public class QuizSubmissionDTO
    {
        public int QuizID { get; set; }
        public int UserID { get; set; }
        public string IPAddress { get; set; }
        public int TimeTaken { get; set; }
        public List<AnswerSubmissionDTO> Answers { get; set; }
    }

    public class AnswerSubmissionDTO
    {
        public int QuestionID { get; set; }
        public string SelectedAnswer { get; set; }

    }

}
