namespace apiTechSkillPro.DTOs
{
    public class UserRegisterDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
        public int RoleID { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}
