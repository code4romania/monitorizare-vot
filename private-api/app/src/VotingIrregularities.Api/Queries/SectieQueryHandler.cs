using MediatR;
using System.Threading.Tasks;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Api.Services;

namespace VotingIrregularities.Api.Queries
{
    public class SectieQueryHandler : IAsyncRequestHandler<ModelSectieQuery, int>
    {
        private readonly ISectieDeVotareService _svService;

        public SectieQueryHandler(ISectieDeVotareService svService)
        {
            _svService = svService;
        }

        public async Task<int> Handle(ModelSectieQuery message)
        {
            return await _svService.GetSingleSectieDeVotare(message.CodJudet, message.NumarSectie);
        }
    }
}
