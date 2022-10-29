using CSharpFunctionalExtensions;
using MediatR;

namespace VoteMonitor.Api.County.Commands
{
    public class DeleteCounty : IRequest<Result>
    {
        public int CountyId { get; }

        public DeleteCounty(int countyId)
        {
            CountyId = countyId;
        }
    }
}
