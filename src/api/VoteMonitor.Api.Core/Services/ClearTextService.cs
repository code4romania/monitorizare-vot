namespace VoteMonitor.Api.Core.Services
{
    public class ClearTextService : IHashService
    {
        public string Salt { get; set; }
        public string GetHash(string clearString) => clearString;
    }
}
