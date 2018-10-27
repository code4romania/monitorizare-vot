using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Domain.UserAggregate
{
    public class RegisterDeviceIdRequestHandler : AsyncRequestHandler<RegisterDeviceId, int>
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;

        public RegisterDeviceIdRequestHandler(VotingContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }
        protected override async Task<int> HandleCore(RegisterDeviceId request)
        {
            try
            {
                var observator = await _context.Observers.SingleAsync(a => a.Id == request.ObserverId);

                observator.MobileDeviceId = request.MobileDeviceId;
                observator.DeviceRegisterDate = DateTime.UtcNow;

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex, ex.Message);
                throw ex;
            }
        }
    }
}