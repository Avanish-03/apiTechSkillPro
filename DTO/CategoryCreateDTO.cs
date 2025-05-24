using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace apiTechSkillPro.DTOs
{
    public class CategoryCreateDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public IFormFile? Image { get; set; }
    }
}
