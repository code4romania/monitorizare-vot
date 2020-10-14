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
    public class RemoveDeviceIdHandler : IRequestHandler<RemoveDeviceIdCommand, bool>
    {
        private readonly VoteMonitorContext _voteMonitorContext;
        private readonly ILogger _logger;

        public RemoveDeviceIdHandler(VoteMonitorContext context, ILogger<ResetDeviceHandler> logger)
        {
            _voteMonitorContext = context;
            _logger = logger;
        }

        public Task<bool> Handle(RemoveDeviceIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var observerQuery = _voteMonitorContext.Observers
                    .Where(o => o.Id == request.IdObserver);

                var observer = observerQuery.FirstOrDefault();

                if (observer == null)
                {
                    return Task.FromResult(false);
                }

                observer.MobileDeviceId = null;

                _voteMonitorContext.Update(observer);
                _voteMonitorContext.SaveChangesAsync(cancellationToken);
                return Task.FromResult(true);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    "Exception caught during removal of mobile device Id of Observer with id " + request.IdObserver,
                    exception);
            }

            return Task.FromResult(false);
        }
    }
}