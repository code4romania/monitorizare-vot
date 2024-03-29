using MediatR;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.PollingStation.Models;
using VoteMonitor.Api.PollingStation.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.PollingStation.Handlers;

public class GetPollingStationByIdHandler : IRequestHandler<GetPollingStationById, GetPollingStationModel>
{
    private readonly VoteMonitorContext _context;
    private readonly ILogger _logger;

    public GetPollingStationByIdHandler(VoteMonitorContext context, ILogger<GetPollingStationByIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task<GetPollingStationModel> Handle(GetPollingStationById request, CancellationToken cancellationToken)
    {
        try
        {
            var pollingStation = _context.PollingStations.Find(request.PollingStationId);

            GetPollingStationModel pollingStationData = null;
            if (pollingStation != null)
            {
                pollingStationData = new GetPollingStationModel()
                {
                    Id = pollingStation.Id,
                    Number = pollingStation.Number,
                    Address = pollingStation.Address,
                    MunicipalityId = pollingStation.MunicipalityId,
                };
            }

            return Task.FromResult(pollingStationData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Polling Station by ID");
            throw;
        }
    }
}
