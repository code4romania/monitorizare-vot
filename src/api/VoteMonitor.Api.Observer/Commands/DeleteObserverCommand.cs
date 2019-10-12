using MediatR;

namespace VoteMonitor.Api.Observer.Commands
{
    public class DeleteObserverCommand : IRequest<bool>
    {
        public int IdObserver { get; set; }
    }
}
