using System.Collections.Generic;
using System.Threading.Tasks;
using VoteMonitor.Api.Location.Models;

namespace VoteMonitor.Api.Location.Services
{
    public interface IPollingStationService
    {
        Task<int> GetPollingStationByCountyCode(int pollingStationNumber, string countyCode);
        Task<int> GetPollingStationByCountyId(int pollingStationNumber, int countyId);
        Task<IEnumerable<CountyPollingStationLimit>> GetPollingStationsAssignmentsForAllCounties();
    }
}