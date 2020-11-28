using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Notification.Models
{
    public class NotificationRegistrationDataModel
    {
        public int? ObserverId { get; set; }

        [Required]
        [MaxLength(256)]
        public string ChannelName { get; set; }

        [Required]
        [MaxLength(512)]
        public string Token { get; set; }
    }
}
