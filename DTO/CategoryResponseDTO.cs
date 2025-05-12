namespace apiTechSkillPro.DTOs
{
    public class CategoryResponseDTO
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }

        // You can also include the list of quizzes, but keep it optional
        public List<QuizResponseDTO> Quizzes { get; set; }
    }
}
