using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Notification.Models
{
    public class NotificationForAllNewModel
    {
        [Required] public string Channel { get; set; }
        [Required] public string From { get; set; }
        [Required] public string Title { get; set; }
        [Required] public string Message { get; set; }
    }
}