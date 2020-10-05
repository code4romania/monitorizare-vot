using System.Collections.Generic;

namespace VoteMonitor.Api.Core.Services
{
    public interface IFirebaseService
    {
        int SendAsync(string from, string title, string message, List<string> recipients);
    }
}
