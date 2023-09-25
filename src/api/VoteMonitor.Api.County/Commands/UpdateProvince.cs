using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Commands;

public record UpdateProvince: IRequest<Result>
{
    public int ProvinceId { get; }
    public string Code { get; }
    public string Name { get; }
    public bool Diaspora { get; }
    public int Order { get; }

    public UpdateProvince(int provinceId, UpdateProvinceRequest county)
    {
        ProvinceId = provinceId;
        Name = county.Name;
        Code = county.Code;
        Order = county.Order;
    }
}
