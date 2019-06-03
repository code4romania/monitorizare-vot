using MediatR;
using System.Threading.Tasks;
using VoteMonitor.Api.Location.Services;
using VotingIrregularities.Api.Models;

namespace VotingIrregularities.Api.Queries
{
    public class SectieQueryHandler : AsyncRequestHandler<ModelSectieQuery, int>
    {
        private readonly IPollingStationService _pollingStationService;

        public SectieQueryHandler(IPollingStationService pollingStationService)
        {
            _pollingStationService = pollingStationService;
        }

        protected override async Task<int> HandleCore(ModelSectieQuery message)
        {
            return await _pollingStationService.GetPollingStationByCountyCode(message.NumarSectie, message.CodJudet);
        }
    }
}
