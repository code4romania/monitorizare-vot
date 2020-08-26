namespace VoteMonitor.Api.Notification.Models
{
    public class NotificationForAllNewModel
    {
        public string Channel { get; set; }
        public string From { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}