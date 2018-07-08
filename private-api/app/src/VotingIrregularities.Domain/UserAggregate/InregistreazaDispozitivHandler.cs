using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Domain.UserAggregate
{
    public class InregistreazaDispozitivHandler : IAsyncRequestHandler<InregistreazaDispozitivCommand, int>
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;

        public InregistreazaDispozitivHandler(VotingContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<int> Handle(InregistreazaDispozitivCommand message)
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
