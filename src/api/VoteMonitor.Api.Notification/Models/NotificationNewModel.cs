using System.Collections.Generic;

namespace VoteMonitor.Api.Notification.Models
{
    public class NotificationNewModel
    {       
        public string Message { get; set; }
        public List<string> recipients { get; set; }

        public string from;
    }
}