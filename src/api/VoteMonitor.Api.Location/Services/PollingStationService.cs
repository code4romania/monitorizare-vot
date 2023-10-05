using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Core.Models;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Location.Services;

public class PollingStationService : IPollingStationService
{
    private readonly VoteMonitorContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger _logger;

    public PollingStationService(VoteMonitorContext context, ICacheService cacheService, ILogger<PollingStationService> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<int> GetPollingStationId(string countyCode, string municipalityCode, int pollingStationNumber)
    {
        try
        {
            var cacheKey = $"polling-station-{countyCode}-{municipalityCode}-{pollingStationNumber}";

            var pollingStationId =  await _cacheService.GetOrSaveDataInCacheAsync<int?>(cacheKey, async () =>
            {
                var municipalityId = _context.Municipalities.FirstOrDefault(c =>c.County.Code == countyCode && c.Code == municipalityCode)?.Id;
                if (municipalityId == null)
                    throw new ArgumentException($"Could not find municipality with code: {countyCode} {municipalityCode}");

                return await GetPollingStationByMunicipalityId(municipalityId.Value, pollingStationNumber);
            });

            return pollingStationId ?? -1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return -1;
    }

    public async Task<int> GetPollingStationByMunicipalityId(int municipalityId, int pollingStationNumber)
    {
        try
        {
            var cacheKey = $"polling-station-{municipalityId}-{pollingStationNumber}";
            var pollingStationId = await _cacheService.GetOrSaveDataInCacheAsync<int?>(cacheKey, async () =>
            {
                var pollingStationIds = await
                    _context.PollingStations
                        .Where(a => a.MunicipalityId == municipalityId && a.Number == pollingStationNumber)
                        .Select(a => a.Id).ToListAsync();

                if (pollingStationIds.Count == 0)
                    throw new ArgumentException($"No Polling station found for: {new { municipalityId, pollingStationNumber }}");


                if (pollingStationIds.Count > 1)
                    throw new ArgumentException($"More than one polling station found for: {new { municipalityId, pollingStationNumber }}");

                return pollingStationIds.Single();
            });

            return pollingStationId ?? -1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred whn searching for polling station");
        }

        return -1;
    }

    public async Task<IEnumerable<CountyPollingStationLimit>> GetPollingStationsAssignmentsForAllCounties(bool? diaspora)
    {
        var cacheKey = "all-polling-stations";

        if (diaspora.HasValue)
        {
            cacheKey = $"polling-station-diaspora-{diaspora.Value}";
        }

        var data = await _cacheService
            .GetOrSaveDataInCacheAsync(cacheKey, async () => await _context.Counties
                .Select(c => new CountyPollingStationLimit
                {
                    Name = c.Name,
                    Code = c.Code,
                    Id = c.Id,
                    Diaspora = c.Diaspora,
                    Order = c.Order,
                    Limit = c.Municipalities.Sum(x => x.PollingStations.Count)
                })
                .Where(c => diaspora == null || c.Diaspora == diaspora)
                .OrderBy(c => c.Order)
                .ToListAsync());

        return data;
    }
}
