using MediatR;

namespace VotingIrregularities.Api.Models
{
    public class SectionModelQuery : IRequest<int>
    {
        public string CountyCode { get; set; }
        public int SectionNumber { get; set; }
    }
}
