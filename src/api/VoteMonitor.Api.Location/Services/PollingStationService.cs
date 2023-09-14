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

            return await _cacheService.GetOrSaveDataInCacheAsync(cacheKey, async () =>
            {
                var municipalityId = _context.Municipalities.FirstOrDefault(c =>c.County.Code == countyCode && c.Code == municipalityCode)?.Id;
                if (municipalityId == null)
                    throw new ArgumentException($"Could not find municipality with code: {countyCode} {municipalityCode}");

                return await GetPollingStationByMunicipalityId(pollingStationNumber, municipalityId.Value);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(new EventId(), ex.Message);
        }

        return -1;
    }

    public async Task<int> GetPollingStationByMunicipalityId(int municipalityId, int pollingStationNumber)
    {
        try
        {
            var cacheKey = $"polling-station-{municipalityId}-{pollingStationNumber}";
            return await _cacheService.GetOrSaveDataInCacheAsync<int>(cacheKey, async () =>
            {
                var pollingStationIds = await
                    _context.PollingStations
                        .Where(a => a.MunicipalityId == municipalityId && a.Number == pollingStationNumber)
                        .Select(a => a.Id).ToListAsync();

                if (pollingStationIds.Count == 0)
                    throw new ArgumentException($"No Polling station found for: {new { municipalityId, pollingStationNumber }}");


                if (pollingStationIds.Count > 1)
                    throw new ArgumentException($"More than one polling station found for: {new { municipalityId, pollingStationIds }}");

                return pollingStationIds.Single();
            });
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
                .Where(c => diaspora == null || c.Diaspora == diaspora)
                .OrderBy(c => c.Order)
                .Select(c => new CountyPollingStationLimit
                {
                    Name = c.Name,
                    Code = c.Code,
                    Id = c.Id,
                    Diaspora = c.Diaspora,
                    Order = c.Order
                }).ToListAsync());

        return data;
    }
}
