using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Commands;

public record UpdateCounty : IRequest<Result>
{
    public int CountyId { get; }
    public string Code { get; }
    public string Name { get; }
    public bool Diaspora { get; }
    public int Order { get; }

    public UpdateCounty(int countyId, UpdateCountyRequest county)
    {
        CountyId = countyId;
        Name = county.Name;
        Code = county.Code;
        Diaspora = county.Diaspora;
        Order = county.Order;
    }
}
