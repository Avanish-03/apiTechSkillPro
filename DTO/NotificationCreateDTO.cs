namespace apiTechSkillPro.DTOs
{
    public class NotificationCreateDTO
    {
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } // Result, Reminder, etc.
        public string Link { get; set; }
    }
}
