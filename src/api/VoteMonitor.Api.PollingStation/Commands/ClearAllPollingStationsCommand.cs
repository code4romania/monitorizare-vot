using CSharpFunctionalExtensions;
using MediatR;

namespace VoteMonitor.Api.PollingStation.Commands
{
    public class ClearAllPollingStationsCommand : IRequest<Result>
    {
        public bool IncludeRelatedData { get; }

        public ClearAllPollingStationsCommand(bool includeRelatedData)
        {
            IncludeRelatedData = includeRelatedData;
        }
    }
}
