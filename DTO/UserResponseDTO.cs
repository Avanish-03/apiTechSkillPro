namespace apiTechSkillPro.DTOs
{
    public class UserResponseDTO
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }  // Optional if you include Role in join
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }
    }
}
