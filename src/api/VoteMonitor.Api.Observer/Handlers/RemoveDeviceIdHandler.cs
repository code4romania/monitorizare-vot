using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers
{
    public class RemoveDeviceIdHandler : IRequestHandler<RemoveDeviceIdCommand>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;

        public RemoveDeviceIdHandler(VoteMonitorContext context, ILogger<RemoveDeviceIdHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Handle(RemoveDeviceIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var observer = await _context.Observers
                    .Where(o => o.Id == request.Id)
                    .FirstAsync(cancellationToken);

                observer.MobileDeviceId = null;
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    "Exception caught during removal of mobile device Id of Observer with id " + request.Id,
                    exception);
                throw;
            }
        }
    }
}
