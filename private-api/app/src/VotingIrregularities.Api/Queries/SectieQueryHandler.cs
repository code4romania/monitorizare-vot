using MediatR;
using System.Threading.Tasks;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Api.Services;

namespace VotingIrregularities.Api.Queries
{
    public class SectieQueryHandler : AsyncRequestHandler<ModelSectieQuery, int>
    {
        private readonly ISectieDeVotareService _svService;

        public SectieQueryHandler(ISectieDeVotareService svService)
        {
            _svService = svService;
        }

        protected override async Task<int> HandleCore(ModelSectieQuery message)
        {
            return await _svService.GetSingleSectieDeVotare(message.CodJudet, message.NumarSectie);
        }
    }
}
