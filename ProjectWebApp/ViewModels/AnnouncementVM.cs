namespace ProjectWebApp.ViewModels
{
    public class AnnouncementVM
    {
        // Form fields
        public string Title { get; set; }
        public string Message { get; set; }

        // Display fields
        public int TotalAnnouncements { get; set; }
        public List<AnnouncementItem> RecentAnnouncements { get; set; }
    }

    public class AnnouncementItem
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
