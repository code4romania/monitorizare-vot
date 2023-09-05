namespace VoteMonitor.Entities;

public class NotificationRegistrationData
{
    public int ObserverId { get; set; }
    public string ChannelName { get; set; }
    public string Token { get; set; }

    public virtual Observer Observer { get; set; }
}
