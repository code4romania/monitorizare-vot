namespace VoteMonitor.Api.Core.Services;

public interface IHashService
{
    string GetHash(string clearString);
}