using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using apiTechSkillPro.Models;

namespace apiTechSkillPro.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [StringLength(255)]
        public string ImageURL { get; set; }

        // Navigation property
        public ICollection<Quiz> Quizzes { get; set; }
    }
}