namespace VoteMonitor.Api.Core.Services;

public interface IFirebaseService
{
    int Send(string from, string title, string message, List<string> recipients);
}