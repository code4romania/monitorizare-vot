using System;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<NotificationRegistrationDataHandler> _logger;

        public NotificationRegistrationDataHandler(VoteMonitorContext context, IFirebaseService firebaseService, IMapper mapper, ILogger<NotificationRegistrationDataHandler> logger)
        {
            _context = context;
            _firebaseService = firebaseService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(NotificationRegistrationDataCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingRegistration =
                    _context.NotificationRegistrationData
                        .FirstOrDefault(data =>
                            data.ObserverId == request.ObserverId && data.ChannelName == request.ChannelName);
                if (existingRegistration != null)
                {
                    existingRegistration.Token = request.Token;
                }
                else
                {
                    var notificationRegistration = new NotificationRegistrationData
                    {
                        ObserverId = request.ObserverId, ChannelName = request.ChannelName, Token = request.Token
                    };
                    await _context.NotificationRegistrationData.AddAsync(notificationRegistration, cancellationToken);
                }

                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error saving notification registration data for Observer: {request.ObserverId}");
                throw;
            }
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
