using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Commands
{
    public class UpdateCounty : IRequest<Result>
    {
        public CountyModel County { get; }

        public UpdateCounty(int countyId, UpdateCountyRequest county)
        {
            County = new CountyModel
            {
                Id = countyId,
                Name = county.Name,
                Code = county.Code,
                Diaspora = county.Diaspora,
                Order = county.Order,
                NumberOfPollingStations = county.NumberOfPollingStations
            };
        }
    }
}