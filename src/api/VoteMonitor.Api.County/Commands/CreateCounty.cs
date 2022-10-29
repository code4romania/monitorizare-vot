using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Commands
{
    public class CreateCounty : IRequest<Result>
    {
        public CountyModel County { get; }

        public CreateCounty(AddOrUpdateCountyRequest county)
        {
            County = new CountyModel
            {
                Name = county.Name,
                Code = county.Code,
                Diaspora = county.Diaspora,
                Order = county.Order,
                NumberOfPollingStations = county.NumberOfPollingStations
            };
        }
    }
}
