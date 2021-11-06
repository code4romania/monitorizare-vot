using VoteMonitor.Api.Core;

namespace VoteMonitor.Api.Notification.Queries
{
    public class NotificationListQuery : PagingModel
    {
        public bool IsOrganizer { get; set; }
        public int? NgoId { get; set; }
    }
}
