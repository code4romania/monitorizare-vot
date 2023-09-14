using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Commands;

public record UpdateMunicipality : IRequest<Result>
{
    public int MunicipalityId { get; }
    public string Code { get; }
    public string Name { get; }
    public int Order { get; }

    public UpdateMunicipality(int municipalityId, UpdateMunicipalityRequest county)
    {
        MunicipalityId = municipalityId;
        Name = county.Name;
        Code = county.Code;
        Order = county.Order;
    }
}
