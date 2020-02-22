using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.PollingStation.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.PollingStation.Handlers
{
    public class CreatePollingStationInfoHandler : IRequestHandler<CreatePollingStationInfo>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CreatePollingStationInfoHandler(VoteMonitorContext context, IMapper mapper, ILogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreatePollingStationInfo request, CancellationToken cancellationToken)
        {
            try
            {
                var pollingStationInfo = _mapper.Map<PollingStationInfo>(request);

                _context.PollingStationInfos.Add(pollingStationInfo);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating polling station info: ", ex);
                throw;
            }
        }
    }
}
