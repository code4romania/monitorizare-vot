using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers
{
    public class ResetDeviceHandler : IRequestHandler<ResetDeviceCommand, int>
    {
        private readonly VoteMonitorContext _voteMonitorContext;
        private readonly ILogger _logger;

        public ResetDeviceHandler(VoteMonitorContext context, ILogger<ResetDeviceHandler> logger)
        {
            _voteMonitorContext = context;
            _logger = logger;
        }

        public Task<int> Handle(ResetDeviceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var observerQuery = _voteMonitorContext.Observers
                    .Where(o => o.Phone == request.PhoneNumber);

                if (!request.Organizer)
                {
                    observerQuery = observerQuery.Where(o => o.IdNgo == request.NgoId);
                }

                var observer = observerQuery.FirstOrDefault();

                if (observer == null)
                {
                    return Task.FromResult(-1);
                }

                observer.DeviceRegisterDate = null;
                observer.MobileDeviceId = null;

                _voteMonitorContext.Update(observer);
                return _voteMonitorContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError("Exception caught during resetting of Observer device for id " + request.PhoneNumber, exception);
            }

            return Task.FromResult(-1);
        }
    }
}
