using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Domain.UserAggregate
{
    public class InregistreazaDispozitivHandler : AsyncRequestHandler<InregistreazaDispozitivCommand, int>
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;

        public InregistreazaDispozitivHandler(VotingContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }
        protected override async Task<int> HandleCore(InregistreazaDispozitivCommand message)
        {
            try
            {
                var observator = await _context.Observers.SingleAsync(a => a.Id == message.IdObservator);

                observator.MobileDeviceId = message.IdDispozitivMobil;
                observator.DeviceRegisterDate = DateTime.UtcNow;

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
