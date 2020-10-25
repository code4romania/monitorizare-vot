using VoteMonitor.Api.Core;

namespace VoteMonitor.Api.Notification.Queries
{
    public class NotificationListQuery : PagingModel
    {
        public bool Organizer { get; set; }
        public int? IdNgo { get; set; }
    }
}
