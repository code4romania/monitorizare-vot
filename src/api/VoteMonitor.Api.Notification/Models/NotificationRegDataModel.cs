namespace VoteMonitor.Api.Notification.Models
{
    public class NotificationRegDataModel
    {       
        public int? ObserverId { get; set; }
        public string ChannelName { get; set; }
        public string Token { get; set; }
    }
}