using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using VoteMonitor.Api.Location.Models;
using VoteMonitor.Api.Location.Queries;
using VoteMonitor.Api.Location.Services;

namespace VoteMonitor.Api.Location.Handlers
{
    public class PollingStationAssignmentHandler : AsyncRequestHandler<PollingStationsAssignmentQuery, IEnumerable<CountyPollingStationLimit>>
    {
        private readonly IPollingStationService _pollingStationService;

        public PollingStationAssignmentHandler(IPollingStationService pollingStationService)
        {
            _pollingStationService = pollingStationService;
        }

        protected override async Task<IEnumerable<CountyPollingStationLimit>> HandleCore(PollingStationsAssignmentQuery message)
        {
            return await _pollingStationService.GetPollingStationsAssignmentsForAllCounties();
        }
    }
}
