using MediatR;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Notification.Models;

namespace VoteMonitor.Api.Notification.Commands;

public class NotificationListCommand : IRequest<ApiListResponse<NotificationModel>>
{
    public bool IsOrganizer { get; set; }
    public int? NgoId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}