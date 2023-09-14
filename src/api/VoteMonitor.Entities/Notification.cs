using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMonitor.Entities;

public class Notification : IIdentifiableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Channel { get; set; }
    public string Body { get; set; }
    public DateTime InsertedAt { get; set; }
    public int SenderAdminId { get; set; }
    public NgoAdmin SenderAdmin { get; set; }
    public virtual ICollection<NotificationRecipient> NotificationRecipients { get; set; }
}
