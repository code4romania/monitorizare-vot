using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Extensions;
using VoteMonitor.Api.Notification.Commands;
using VoteMonitor.Api.Notification.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Notification.Handlers;

public class NotificationListQueryHandler : QueryExtension, IRequestHandler<NotificationListCommand, ApiListResponse<NotificationModel>>
{
    private readonly VoteMonitorContext _context;
    private readonly ILogger _logger;

    public NotificationListQueryHandler(VoteMonitorContext context, ILogger<NotificationListQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiListResponse<NotificationModel>> Handle(NotificationListCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Searching for Notifications with the following filters (IsOrganizer, NgoId): {request.IsOrganizer}, {request.NgoId}");

        IQueryable<Entities.Notification> notifications = _context.Notifications
            .Include(o => o.SenderAdmin).ThenInclude(o => o.Ngo)
            .Include(o => o.NotificationRecipients);

        if (!request.IsOrganizer)
        {
            notifications = notifications.Where(o => o.SenderAdmin.IdNgo == request.NgoId);
        }

        var count = await notifications.CountAsync(cancellationToken);

        var requestedPageNotifications = GetPagedQuery(notifications, request.Page, request.PageSize)
            .ToList()
            .Select(x => new NotificationModel()
            {
                Id = x.Id,
                Channel = x.Channel,
                Body = x.Body,
                SenderId = x.SenderAdmin.Id,
                SenderAccount = x.SenderAdmin.Account,
                InsertedAt = x.InsertedAt,
                SenderIdNgo = x.SenderAdmin.IdNgo,
                SenderNgoName = x.SenderAdmin.Ngo.Name,
                Title = x.Title,
                SentObserverIds = x.NotificationRecipients.Select(o => o.ObserverId).ToList()
            });

        return new ApiListResponse<NotificationModel>
        {
            TotalItems = count,
            Data = requestedPageNotifications.ToList(),
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
