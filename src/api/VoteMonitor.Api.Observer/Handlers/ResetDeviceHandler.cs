using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers
{
    public class ResetDeviceHandler : AsyncRequestHandler<ResetDeviceCommand, int>
    {
        private readonly VoteMonitorContext _voteMonitorContext;
        private readonly ILogger _logger;

        public ResetDeviceHandler(VoteMonitorContext context, ILogger logger)
        {
            _voteMonitorContext = context;
            _logger = logger;
        }

        protected override Task<int> HandleCore(ResetDeviceCommand request)
        {
            try
            {
                var observer = _voteMonitorContext.Observers
                    .First(o => o.Phone == request.PhoneNumber &&
                                o.IdNgo == request.IdNgo);

                if (observer == null)
                    return Task.FromResult(-1);

                observer.DeviceRegisterDate = null;
                observer.MobileDeviceId = null;

                _voteMonitorContext.Update(observer);
                return _voteMonitorContext.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError("Exception caught during resetting of Observer device for id " + request.PhoneNumber, exception);
            }

            return Task.FromResult(-1);
        }
    }
}
