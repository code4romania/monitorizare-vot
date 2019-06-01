using System.Collections.Generic;
using MediatR;
using System.Threading.Tasks;
using VotingIrregularities.Api.Services;
using VotingIrregularities.Api.Models;

namespace VotingIrregularities.Api.Queries
{
    public class PollingStationsAssignmentQuery : IRequest<IEnumerable<CountyPollingStationLimit>>
    {
    }

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
