using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.County.Commands;
using VoteMonitor.Api.County.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.County.Handlers;

public class CacheHandler : IRequestHandler<PrefillCache>
{
    private readonly ICacheService _cacheService;
    private readonly VoteMonitorContext _context;

    public CacheHandler(ICacheService cacheService, VoteMonitorContext context)
    {
        _cacheService = cacheService;
        _context = context;
    }
    public async Task Handle(PrefillCache request, CancellationToken cancellationToken)
    {
        var provinces = await _context
             .Provinces
             .Include(x => x.Counties)
             .ThenInclude(x => x.Municipalities)
             .ToListAsync(cancellationToken: cancellationToken);


        await _cacheService.SaveObjectSafeAsync("provinces", provinces
            .OrderBy(x => x.Order)
            .Select(x => new ProvinceModel()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Order = x.Order
            }).ToList());

        foreach (var province in provinces)
        {
            await _cacheService.SaveObjectSafeAsync($"province-{province.Code}/counties", province.Counties
                .OrderBy(c => c.Order)
                .Select(x => new CountyModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    ProvinceCode = province.Code,
                    Diaspora = x.Diaspora,
                    Name = x.Name,
                    NumberOfPollingStations = x.Municipalities.Sum(x=>x.PollingStations.Count),
                    Order = x.Order
                }).ToList());

            foreach (var county in province.Counties)
            {
                await _cacheService.SaveObjectSafeAsync(
                   $"county-{county.Code}/municipalities", county.Municipalities
                       .OrderBy(m => m.Order)
                       .Select(m => new MunicipalityModel
                       {
                           Id = m.Id,
                           Code = m.Code,
                           CountyCode = county.Code,
                           Name = m.Name,
                           Order = m.Order,
                           NumberOfPollingStations = m.PollingStations.Count
                       })
                       .ToList());
            }
        }

    }
}
