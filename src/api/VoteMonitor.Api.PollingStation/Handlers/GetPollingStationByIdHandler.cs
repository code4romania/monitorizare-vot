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
    public class GetPollingStationByIdHandler : IRequestHandler<GetPollingStationById, Models.GetPollingStation>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetPollingStationByIdHandler(VoteMonitorContext context, IMapper mapper, ILogger<GetPollingStationByIdHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<Models.GetPollingStation> Handle(GetPollingStationById request, CancellationToken cancellationToken)
        {
            try
            {
                var pollingStation = _context.PollingStations.Find(request.Id);

                Models.GetPollingStation pollingStationData = null;
                if (pollingStation != null)
                {
                    pollingStationData = _mapper.Map<Models.GetPollingStation>(pollingStation);
                }

                return Task.FromResult(pollingStationData);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "Error retrieving Polling Station by ID", ex);
                throw;
            }
        }
    }
}
