using System.Threading.Tasks;

namespace VotingIrregularities.Api.Services
{
    public interface IVotingSectionService
    {
        Task<int> GetSingleVotingSection(string countyCode, int sectionNumber);
        Task<int> GetSingleVotingSection(int countyId, int sectionNumber);
    }
}
