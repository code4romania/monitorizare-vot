namespace VoteMonitor.Api.Notification.Models
{
    public class NotificationRegistrationDataModel
    {       
        public int? ObserverId { get; set; }
        public string ChannelName { get; set; }
        public string Token { get; set; }
    }
}