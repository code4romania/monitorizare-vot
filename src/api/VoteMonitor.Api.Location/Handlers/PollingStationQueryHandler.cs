using MediatR;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Location.Queries;

namespace VoteMonitor.Api.Location.Handlers;

public class PollingStationQueryHandler : IRequestHandler<GetPollingStationId, int>
{
    private readonly IPollingStationService _pollingStationService;

    public PollingStationQueryHandler(IPollingStationService pollingStationService)
    {
        _pollingStationService = pollingStationService;
    }

    public async Task<int> Handle(GetPollingStationId message, CancellationToken cancellationToken)
    {
        return await _pollingStationService.GetPollingStationId(message.CountyCode, message.MunicipalityCode, message.PollingStationNumber);
    }
}
