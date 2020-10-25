using MediatR;

namespace VoteMonitor.Api.Observer.Queries
{
    public class CheckObserverExists : IRequest<bool>
    {
        public int Id { get; set; }
    }
}