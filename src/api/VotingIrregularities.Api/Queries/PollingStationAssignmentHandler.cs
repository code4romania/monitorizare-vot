using System.Collections.Generic;
using MediatR;
using System.Threading.Tasks;
using VotingIrregularities.Api.Services;

namespace VotingIrregularities.Api.Queries
{
    public class PollingStationsAssignmentQuery : IRequest<Dictionary<string, int>>
    {
    }

    public class PollingStationAssignmentHandler : AsyncRequestHandler<PollingStationsAssignmentQuery, Dictionary<string, int>>
    {
        private readonly IPollingStationService _pollingStationService;

        public PollingStationAssignmentHandler(IPollingStationService pollingStationService)
        {
            _pollingStationService = pollingStationService;
        }

        protected override async Task<Dictionary<string,int>> HandleCore(PollingStationsAssignmentQuery message)
        {
            return await _pollingStationService.GetPollingStationsAssignmentsForAllCounties();
        }
    }
}
