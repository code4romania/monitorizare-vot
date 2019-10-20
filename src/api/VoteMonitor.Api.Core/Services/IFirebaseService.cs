using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VoteMonitor.Api.Core.Services
{
    public interface IFirebaseService
    {
        int SendAsync(String from, String title, String message, IList<string> recipients);
    }
}
