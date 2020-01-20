using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Commands
{
    public class UpdateCounty : IRequest<Result>
    {
        public CountyModel County { get; }
        public int CountyId { get; }

        public UpdateCounty(int countyId, CountyModel county)
        {
            CountyId = countyId;
            County = county;
        }
    }
}