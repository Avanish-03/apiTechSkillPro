namespace apiTechSkillPro.DTOs
{
    public class NotificationResponseDTO
    {
        public int NotificationID { get; set; }
        public int UserID { get; set; }
        public string UserFullName { get; set; } // Adding User Full Name
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
        public string Link { get; set; }
    }
}
