using CSharpFunctionalExtensions;
using MediatR;

namespace VoteMonitor.Api.PollingStation.Commands
{
    public class ClearAllPollingStationsCommand : IRequest<Result>
    {
        internal bool IncludeRelatedData;

        public ClearAllPollingStationsCommand(bool includeRelatedData)
        {
            IncludeRelatedData = includeRelatedData;
        }
    }
}
