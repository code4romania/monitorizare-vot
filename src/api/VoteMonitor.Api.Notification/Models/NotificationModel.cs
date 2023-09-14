namespace VoteMonitor.Api.Notification.Models;

public class NotificationModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Channel { get; set; }
    public string Body { get; set; }
    public DateTime InsertedAt { get; set; }
    //
    public int SenderId { get; set; }
    public int SenderIdNgo { get; set; }
    public string SenderNgoName { get; set; }
    public string SenderAccount { get; set; }
    //
    public ICollection<int> SentObserverIds { get; set; }
}
