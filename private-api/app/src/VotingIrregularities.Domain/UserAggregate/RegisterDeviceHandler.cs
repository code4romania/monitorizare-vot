using MediatR;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Domain.UserAggregate
{
    public class RegisterDeviceHandler : IAsyncRequestHandler<RegisterDeviceCommand, int>
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;

        public RegisterDeviceHandler(VotingContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<int> Handle(RegisterDeviceCommand message)
        {
            try
            {
                var observer = await _context.Observer.SingleAsync(a => a.ObserverId == message.ObserverId);

                observer.MobileDeviceId = message.MobileDeviceId;
                observer.DeviceRegistrationDate = DateTime.UtcNow;

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex, ex.Message);
            }

            return -1;
        }
    }
}
