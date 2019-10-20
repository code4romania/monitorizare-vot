using System.Collections.Generic;

namespace VoteMonitor.Api.Notification.Models
{
    public class NotificationNewModel
    {
        public string Channel { get; set; }
        public string From { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public List<string> Recipients { get; set; }
    }
}