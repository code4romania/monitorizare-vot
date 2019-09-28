using VoteMonitor.Api.Core.Services;

namespace VotingIrregularities.Api.Services
{
    public class ClearTextService : IHashService
    {
        public string Salt { get; set; }
        public string GetHash(string clearString) => clearString;
    }
}