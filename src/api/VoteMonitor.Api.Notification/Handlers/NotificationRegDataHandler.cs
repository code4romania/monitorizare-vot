using MediatR;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Entities;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System;

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
            NotificationRegData notificationReg = new NotificationRegData();

            notificationReg.ObserverId = request.ObserverId;
            notificationReg.ChannelName = request.ChannelName;
            notificationReg.Token = request.Token;

            _context.NotificationRegData.Add(notificationReg);

            return _context.SaveChangesAsync();
        }

        public Task<int> Handle(NewNotificationCommand request, CancellationToken cancellationToken)
        {
            List<string> targetFCMTokens = new List<string>();

            foreach (string observer in request.Recipients)
            {
                var regDataResult = _context.NotificationRegData.AsQueryable().Where(regData => regData.ObserverId == Int32.Parse(observer))
               .Where(regData => regData.ChannelName == request.Channel).First();
                targetFCMTokens.Add(regDataResult.Token);
            }

            int response = 0;

            if(targetFCMTokens.Count > 0)
            {
                response = _firebaseService.SendAsync(request.From, request.Title, request.Message, targetFCMTokens);
            }            

            return Task.FromResult(response);
        }
    }
}