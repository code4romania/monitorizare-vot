namespace VoteMonitor.Entities
{
    public partial class NotificationRegistrationData
    {
        public int ObserverId { get; set; }
        public string ChannelName { get; set; }
        public string Token { get; set; }

        public virtual Observer Observer { get; set; }
    }
}
