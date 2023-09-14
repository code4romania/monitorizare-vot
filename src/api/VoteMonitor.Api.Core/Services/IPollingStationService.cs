using VoteMonitor.Api.Core.Models;

namespace VoteMonitor.Api.Core.Services;

public interface IPollingStationService
{
    Task<int> GetPollingStationId(string countyCode, string municipalityCode, int pollingStationNumber);
    Task<int> GetPollingStationByMunicipalityId(int municipalityId, int pollingStationNumber);
    Task<IEnumerable<CountyPollingStationLimit>> GetPollingStationsAssignmentsForAllCounties(bool? diaspora);
}
