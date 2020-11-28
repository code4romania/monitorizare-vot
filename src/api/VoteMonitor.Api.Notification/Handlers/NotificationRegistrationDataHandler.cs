using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Notification.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Notification.Handlers
{
    public class NotificationRegistrationDataHandler :
        IRequestHandler<NotificationRegistrationDataCommand, int>,
        IRequestHandler<NewNotificationCommand, int>,
        IRequestHandler<SendNotificationToAll, int>
    {
        private readonly VoteMonitorContext _context;
        private readonly IFirebaseService _firebaseService;
        private readonly IMapper _mapper;

        public NotificationRegistrationDataHandler(VoteMonitorContext context, IFirebaseService firebaseService, IMapper mapper)
        {
            _context = context;
            _firebaseService = firebaseService;
            _mapper = mapper;
        }

        public Task<int> Handle(NotificationRegistrationDataCommand request, CancellationToken cancellationToken)
        {
            var existingRegistration =
                _context.NotificationRegistrationData
                .FirstOrDefault(data => data.ObserverId == request.ObserverId && data.ChannelName == request.ChannelName);

            if (existingRegistration != null)
            {
                existingRegistration.Token = request.Token;
            }
            else
            {
                var notificationReg = new NotificationRegistrationData
                {
                    ObserverId = request.ObserverId,
                    ChannelName = request.ChannelName,
                    Token = request.Token
                };

                _context.NotificationRegistrationData.Add(notificationReg);
            }

            return _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> Handle(NewNotificationCommand request, CancellationToken cancellationToken)
        {
            var observerIds = request.Recipients.Select(observer => int.Parse(observer)).ToList();
            
            var targetFcmTokens =_context.NotificationRegistrationData
                        .Where(regData => observerIds.Contains(regData.ObserverId))
                        .Where(regData => regData.ChannelName.ToLower() == request.Channel.ToLower())
                        .Select(regDataResult => regDataResult.Token)
                        .Where(token => !string.IsNullOrWhiteSpace(token))
                        .ToList();

            var response = 0;

            if (targetFcmTokens.Count > 0)
            {
                response = _firebaseService.SendAsync(request.From, request.Title, request.Message, targetFcmTokens);
            }

            var notification = _mapper.Map<Entities.Notification>(request);

            _context.Notifications.AddRange(notification);
            await _context.SaveChangesAsync(cancellationToken);
            return response;
        }

        public async Task<int> Handle(SendNotificationToAll request, CancellationToken cancellationToken)
        {
            var targetFcmTokens = _context.NotificationRegistrationData
                .AsNoTracking()
                .Where(x => x.ChannelName == request.Channel)
                .Select(regDataResult => regDataResult.Token)
                .ToList();

            var response = 0;

            if (targetFcmTokens.Count > 0)
            {
                response = _firebaseService.SendAsync(request.From, request.Title, request.Message, targetFcmTokens);
            }

            var observerIds = _context.NotificationRegistrationData
                .AsNoTracking()
                .Where(x => x.ChannelName == request.Channel)
                .Select(regDataResult => regDataResult.ObserverId.ToString())
                .ToList();

            var notification = _mapper.Map<Entities.Notification>(new NewNotificationCommand
            {
                Channel = request.Channel,
                Title = request.Title,
                Message = request.Message,
                From = request.From,
                Recipients = observerIds,
                SenderAdminId = request.SenderAdminId
            });

            _context.Notifications.AddRange(notification);
            await _context.SaveChangesAsync(cancellationToken);
            return response;
        }
    }
}