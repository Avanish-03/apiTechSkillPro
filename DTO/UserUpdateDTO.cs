namespace apiTechSkillPro.DTOs
{
    public class UserUpdateDTO
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string ProfileImage { get; set; }
        public bool IsActive { get; set; }
    }
}
