using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMonitor.Entities;

public class NotificationRecipient
{
    [Key, Column(Order = 1)]
    public int ObserverId { get; set; }
    [Key, Column(Order = 2)]
    public int NotificationId { get; set; }
    public Observer Observer { get; set; }
    public Notification Notification { get; set; }

}
