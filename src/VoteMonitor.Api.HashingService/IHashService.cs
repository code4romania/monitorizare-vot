namespace VoteMonitor.Api.HashingService
{
    public interface IHashService
    {
        string Salt { get; }
        string GetHash(string clearString);
    }
}
