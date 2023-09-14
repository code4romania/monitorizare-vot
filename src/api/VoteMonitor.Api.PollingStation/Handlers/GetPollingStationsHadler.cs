using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.PollingStation.Models;
using VoteMonitor.Api.PollingStation.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.PollingStation.Handlers;

public class GetPollingStationsHandler : IRequestHandler<GetPollingStations, IEnumerable<GetPollingStationModel>>
{
    private readonly VoteMonitorContext _context;
    private readonly ILogger _logger;

    public GetPollingStationsHandler(VoteMonitorContext context, ILogger<GetPollingStationsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<GetPollingStationModel>> Handle(GetPollingStations request, CancellationToken cancellationToken)
    {
        var skip = (request.Page - 1) * request.PageSize;
        var take = request.PageSize;

        try
        {
            var iQueryable = CreateQuery(request);

            var pollingStations = await iQueryable
                .Skip(skip)
                .Take(take)
                .Select(x => new GetPollingStationModel()
                {
                    Id = x.Id,
                    Number = x.Number,
                    Address = x.Address,
                    MunicipalityId = x.MunicipalityId
                })
                .ToListAsync(cancellationToken);

            return pollingStations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Polling Stations");
            throw;
        }
    }

    private IQueryable<Entities.PollingStation> CreateQuery(GetPollingStations request)
    {
        IQueryable<Entities.PollingStation> iQueryable = _context.PollingStations;

        if (request.CountyId > 0)
        {
            iQueryable = iQueryable.Where(p => p.Municipality.County.Id == request.CountyId);
        }

        return iQueryable;
    }
}
