using MediatR;

namespace VoteMonitor.Api.PollingStation.Commands
{
    public class ClearAllPollingStationsCommand : IRequest<int>
    {
    }
}
