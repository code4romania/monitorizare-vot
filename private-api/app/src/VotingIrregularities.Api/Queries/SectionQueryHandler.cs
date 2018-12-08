using MediatR;
using System.Threading.Tasks;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Api.Services;

namespace VotingIrregularities.Api.Queries
{
    public class SectionQueryHandler : IAsyncRequestHandler<SectionModelQuery, int>
    {
        private readonly IVotingSectionService _svService;

        public SectionQueryHandler(IVotingSectionService svService)
        {
            _svService = svService;
        }

        public async Task<int> Handle(SectionModelQuery message)
        {
            return await _svService.GetSingleVotingSection(message.CountyCode, message.SectionNumber);
        }
    }
}
