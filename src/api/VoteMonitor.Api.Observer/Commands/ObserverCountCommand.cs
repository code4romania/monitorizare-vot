using MediatR;

namespace VoteMonitor.Api.Observer.Commands {
    public class ObserverCountCommand : IRequest<int> {
        public int IdNgo { get; set; }
    }
}
