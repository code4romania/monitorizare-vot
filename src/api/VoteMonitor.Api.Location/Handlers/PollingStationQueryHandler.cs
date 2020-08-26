using MediatR;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Location.Queries;
using VoteMonitor.Api.Location.Services;

namespace VoteMonitor.Api.Location.Handlers
{
    public class PollingStationQueryHandler : IRequestHandler<PollingStationQuery, int>
    {
        private readonly IPollingStationService _pollingStationService;

        public PollingStationQueryHandler(IPollingStationService pollingStationService)
        {
            _pollingStationService = pollingStationService;
        }

        public async Task<int> Handle(PollingStationQuery message, CancellationToken cancellationToken)
        {
            return await _pollingStationService.GetPollingStationByCountyCode(message.IdPollingStation, message.CountyCode);
        }
    }
}
