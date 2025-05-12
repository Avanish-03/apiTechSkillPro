using System.ComponentModel.DataAnnotations;

namespace apiTechSkillPro.DTOs
{
    public class CategoryUpdateDTO
    {
        public int CategoryID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageURL { get; set; }
    }
}
