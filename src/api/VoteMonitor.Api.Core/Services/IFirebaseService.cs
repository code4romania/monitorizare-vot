using System;
using System.Collections.Generic;
using System.Text;

namespace VoteMonitor.Api.Core.Services
{
    public interface IFirebaseService
    {
        bool send(String message, List<String> recipients);
    }
}
