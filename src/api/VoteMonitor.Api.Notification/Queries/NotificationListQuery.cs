using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Models;

namespace VoteMonitor.Api.Notification.Queries
{
    public class NotificationListQuery : PagingModel
    {
        public UserType UserType { get; set; }
        public int? IdNgo { get; set; }
    }
}
