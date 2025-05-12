using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;  // Add this for JsonIgnore

namespace apiTechSkillPro.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }

        public string Description { get; set; }
        public string Permissions { get; set; } // JSON string

        // Navigation property
        [JsonIgnore]  // Prevent circular reference
        public ICollection<User> Users { get; set; }
    }
}
