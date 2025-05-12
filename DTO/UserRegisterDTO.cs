namespace apiTechSkillPro.DTOs
{
    public class UserRegisterDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }  // Plain password (hash it in backend)
        public int RoleID { get; set; }
        public string ProfileImage { get; set; }
    }
}
