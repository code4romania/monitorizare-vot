using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Location.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Location.Handlers;

public class PrefillCacheHandler : IRequestHandler<PrefillCache>
{
    private readonly ICacheService _cacheService;
    private readonly VoteMonitorContext _context;

    public PrefillCacheHandler(ICacheService cacheService, VoteMonitorContext context)
    {
        _cacheService = cacheService;
        _context = context;
    }

    public async Task Handle(PrefillCache request, CancellationToken cancellationToken)
    {
        var pollingStations = await _context.PollingStations.AsNoTracking()
            .Select(ps =>
                new
                {
                    ps.Id,
                    MunicipalityCode = ps.Municipality.Code,
                    CountyCode = ps.Municipality.County.Code,
                    ps.Number
                })
            .ToListAsync(cancellationToken: cancellationToken);


        var tasks = pollingStations.Select(ps => _cacheService.SaveObjectSafeAsync($"polling-station-{ps.CountyCode}-{ps.MunicipalityCode}-{ps.Number}", ps.Id));

        await Task.WhenAll(tasks);
    }
}
