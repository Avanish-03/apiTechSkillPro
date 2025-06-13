namespace apiTechSkillPro.DTO
{
    public class QuestionDTO
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
    }
}
