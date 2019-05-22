using System.Collections.Generic;
using System.Threading.Tasks;

namespace VotingIrregularities.Api.Services
{
    public interface IPollingStationService
    {
        Task<int> GetPollingStationByCountyCode(int pollingStationNumber, string countyCode);
        Task<int> GetPollingStationByCountyId(int pollingStationNumber, int countyId);
        Task<Dictionary<string, int>> GetPollingStationsAssignmentsForAllCounties();
    }
}
