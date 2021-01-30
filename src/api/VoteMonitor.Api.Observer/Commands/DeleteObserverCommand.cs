using MediatR;

namespace VoteMonitor.Api.Observer.Commands
{
    public class DeleteObserverCommand : IRequest<bool>
    {
        public int ObserverId { get; }

        public DeleteObserverCommand(int observerId)
        {
            ObserverId = observerId;
        }
    }
}
