using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Auth.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Auth.Handlers
{
    public class RegisterDeviceIdRequestHandler : IRequestHandler<RegisterDeviceId, int>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;

        public RegisterDeviceIdRequestHandler(VoteMonitorContext context, ILogger<RegisterDeviceIdRequestHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<int> Handle(RegisterDeviceId request, CancellationToken cancellationToken)
        {
            try
            {
                var observator = await _context.Observers.SingleAsync(a => a.Id == request.ObserverId, cancellationToken: cancellationToken);

                observator.MobileDeviceIdType = request.MobileDeviceIdType;
                observator.MobileDeviceId = request.MobileDeviceId;
                observator.DeviceRegisterDate = DateTime.UtcNow;

                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex, ex.Message);
                throw ex;
            }
        }
    }
}