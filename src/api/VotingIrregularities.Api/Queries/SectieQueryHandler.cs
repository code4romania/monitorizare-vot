using System.Threading;
using MediatR;
using System.Threading.Tasks;
using VoteMonitor.Api.Location.Services;
using VotingIrregularities.Api.Models;

namespace VotingIrregularities.Api.Queries
{
    public class SectieQueryHandler : IRequestHandler<ModelSectieQuery, int>
    {
        private readonly IPollingStationService _pollingStationService;

        public SectieQueryHandler(IPollingStationService pollingStationService)
        {
            _pollingStationService = pollingStationService;
        }

        public async Task<int> Handle(ModelSectieQuery message, CancellationToken cancellationToken)
        {
            return await _pollingStationService.GetPollingStationByCountyCode(message.NumarSectie, message.CodJudet);
        }
    }
}
