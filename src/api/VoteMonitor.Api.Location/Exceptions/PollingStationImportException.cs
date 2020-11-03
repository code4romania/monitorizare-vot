using System;

namespace VoteMonitor.Api.Location.Exceptions
{
    public class PollingStationImportException : Exception
    {
        public PollingStationImportException(string message) : base(message)
        {
        }
    }
}
