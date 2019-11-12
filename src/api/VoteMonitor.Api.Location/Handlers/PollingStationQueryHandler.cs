using MediatR;
using System.Threading.Tasks;
using VoteMonitor.Api.Location.Queries;
using VoteMonitor.Api.Location.Services;

namespace VoteMonitor.Api.Location.Handlers
{
    public class PollingStationQueryHandler : AsyncRequestHandler<PollingStationQuery, int>
    {
        private readonly IPollingStationService _pollingStationService;

        public PollingStationQueryHandler(IPollingStationService pollingStationService)
        {
            _pollingStationService = pollingStationService;
        }

        protected override async Task<int> HandleCore(PollingStationQuery message)
        {
            return await _pollingStationService.GetPollingStationByCountyCode(message.IdPollingStation, message.CountyCode);
        }
    }
}
