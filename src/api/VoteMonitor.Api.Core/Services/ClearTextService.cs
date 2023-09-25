namespace VoteMonitor.Api.Core.Services;

public class ClearTextService : IHashService
{
    public string GetHash(string clearString) => clearString;
}