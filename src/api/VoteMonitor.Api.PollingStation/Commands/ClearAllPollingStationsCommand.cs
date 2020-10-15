using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.PollingStation.Models;

namespace VoteMonitor.Api.PollingStation.Commands
{
    public class ClearAllPollingStationsCommand : IRequest<Result>
    {
        internal bool IncludeRelatedData;

        public ClearAllPollingStationsCommand(bool includeRelatedData)
        {
            this.IncludeRelatedData = includeRelatedData;
        }
    }
}
