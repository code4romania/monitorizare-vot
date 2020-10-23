using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Observer.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers
{
    public class CheckObserverExistsHandler : IRequestHandler<CheckObserverExists, bool>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;

        public CheckObserverExistsHandler(VoteMonitorContext context, ILogger<CheckObserverExistsHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(CheckObserverExists request, CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Observers.AnyAsync(p => p.Id == request.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving observer: ", ex);
                throw;
            }
        }
    }
}