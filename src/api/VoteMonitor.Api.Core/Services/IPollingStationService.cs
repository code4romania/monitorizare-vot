using VoteMonitor.Api.Core.Models;

namespace VoteMonitor.Api.Core.Services;

public interface IPollingStationService
{
    Task<int> GetPollingStationByCountyCode(int pollingStationNumber, string countyCode);
    Task<int> GetPollingStationByCountyId(int pollingStationNumber, int countyId);
    Task<IEnumerable<CountyPollingStationLimit>> GetPollingStationsAssignmentsForAllCounties(bool? diaspora);
}
