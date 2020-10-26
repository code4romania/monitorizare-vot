using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Extensions;
using VoteMonitor.Api.Notification.Commands;
using VoteMonitor.Api.Notification.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Notification.Handlers
{
    public class NotificationListQueryHandler : QueryExtension, IRequestHandler<NotificationListCommand, ApiListResponse<NotificationModel>>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public NotificationListQueryHandler(VoteMonitorContext context, ILogger<NotificationListQueryHandler> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ApiListResponse<NotificationModel>> Handle(NotificationListCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Searching for Notifications with the following filters (Organizer, IdNgo): {request.Organizer}, {request.IdNgo}");

            IQueryable<Entities.Notification> notifications = _context.Notifications
                .Include(o => o.SenderAdmin).ThenInclude(o => o.Ngo)
                .Include(o => o.NotificationRecipients);

            if (!request.Organizer)
            {
                notifications = notifications.Where(o => o.SenderAdmin.IdNgo == request.IdNgo);
            }

            var count = await notifications.CountAsync(cancellationToken);

            var requestedPageNotifications = GetPagedQuery(notifications, request.Page, request.PageSize)
                .ToList()
                .Select(_mapper.Map<NotificationModel>);

            return new ApiListResponse<NotificationModel>
            {
                TotalItems = count,
                Data = requestedPageNotifications.ToList(),
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
