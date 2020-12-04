using MediatR;

namespace VoteMonitor.Api.Observer.Commands
{
    public class EditObserverCommand : IRequest<int>
    {
        public int ObserverId { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Pin { get; set; }
    }
}
