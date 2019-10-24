using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Notification.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Notification.Handlers
{
    public class NotificationRegDataHandler :
        IRequestHandler<NotificationRegDataCommand, int>,
        IRequestHandler<NewNotificationCommand, int>
    {
        private readonly VoteMonitorContext _context;
        private readonly IFirebaseService _firebaseService;

        public NotificationRegDataHandler(VoteMonitorContext context, IFirebaseService firebaseService)
        {
            _context = context;
            _firebaseService = firebaseService;
        }

        public Task<int> Handle(NotificationRegDataCommand request, CancellationToken cancellationToken)
        {
            var notificationReg = new NotificationRegData
            {
                ObserverId = request.ObserverId, ChannelName = request.ChannelName, Token = request.Token
            };

            _context.NotificationRegData.Add(notificationReg);

            return _context.SaveChangesAsync(cancellationToken);
        }

        public Task<int> Handle(NewNotificationCommand request, CancellationToken cancellationToken)
        {
            var targetFcmTokens = request.Recipients
                    .Select(observer => _context.NotificationRegData.AsQueryable()
                                                                    .Where(regData => regData.ObserverId == int.Parse(observer))
                    .First(regData => regData.ChannelName == request.Channel))
                    .Select(regDataResult => regDataResult.Token)
                    .ToList();

            var response = 0;

            if(targetFcmTokens.Count > 0)
                response = _firebaseService.SendAsync(request.From, request.Title, request.Message, targetFcmTokens);

            return Task.FromResult(response);
        }
    }
}