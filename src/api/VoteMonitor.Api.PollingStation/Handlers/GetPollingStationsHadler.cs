using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.PollingStation.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.PollingStation.Handlers
{
    public class GetPollingStationsHandler : IRequestHandler<GetAllPollingStations, IEnumerable<Models.PollingStation>>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetPollingStationsHandler(VoteMonitorContext context, IMapper mapper, ILogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Models.PollingStation>> Handle(GetAllPollingStations request, CancellationToken cancellationToken)
        {
            try
            {
                var pollingStations = await _context.PollingStations
                    .Select(m => _mapper.Map<Models.PollingStation>(m))
                    .ToListAsync(cancellationToken);

                return pollingStations;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "Error retrieving Polling Stations", ex);
                throw;
            }
        }
    }
}
