namespace VoteMonitor.Api.HashingService
{
    internal class ClearTextService : IHashService
    {
        public string Salt { get; }
        public string GetHash(string clearString) => clearString;
    }
}
