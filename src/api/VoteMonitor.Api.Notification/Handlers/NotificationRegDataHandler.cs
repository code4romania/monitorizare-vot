using MediatR;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Entities;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace VoteMonitor.Api.Observer.Handlers
{
    public class NotificationRegDataHandler :
        IRequestHandler<NotificationRegDataCommand, int>,
        IRequestHandler<NewNotificationCommand, int>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;
        private IHashService _hashService;
        private IFirebaseService _firebaseService;

        public NotificationRegDataHandler(VoteMonitorContext context, ILogger logger, IHashService hashService, IFirebaseService firebaseService)
        {
            _context = context;
            _logger = logger;
            _hashService = hashService;
            _firebaseService = firebaseService;
        }

        public Task<int> Handle(NotificationRegDataCommand request, CancellationToken cancellationToken)
        {
            var notificationRegData = _context.NotificationsRegData.AsQueryable();

            notificationRegData = notificationRegData.Where(regData => regData.ObeserverId == request.ObserverId)
                .Where(regData => regData.ChannelName == request.ChannelName);

            NotificationRegData notificationReg = new NotificationRegData();

            notificationReg.ObeserverId = request.ObserverId;
            notificationReg.ChannelName = request.ChannelName;
            notificationReg.Token = request.Token;

            _context.NotificationsRegData.Add(notificationReg);

            return _context.SaveChangesAsync();
        }

        public Task<int> Handle(NewNotificationCommand request, CancellationToken cancellationToken)
        {
            _firebaseService.send(request.Message, request.recipients);

            return Task.FromResult(0);
        }
    }
}