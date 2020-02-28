using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Queries
{
    public class GetCounty : IRequest<Result<CountyModel>>
    {
        public GetCounty(int countyId)
        {
            CountyId = countyId;
        }

        public int CountyId { get;  }
    }
}
