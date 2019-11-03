using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VoteMonitor.Entities
{
    public partial class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Channel { get; set; }
        public string Body { get; set; }
        public DateTime InsertedAt { get; set; }
        public virtual ICollection<NotificationRecipient> NotificationRecipients { get; set; }
    }

    public partial class NotificationRecipient
    {
        [Key, Column(Order = 1)]
        public int ObserverId { get; set; }
        [Key, Column(Order = 2)]
        public int NotificationId { get; set; }
        public Observer Observer { get; set; }
        public Notification Notification { get; set; }

    }
}
