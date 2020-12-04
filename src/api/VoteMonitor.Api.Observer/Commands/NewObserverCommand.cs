using MediatR;

namespace VoteMonitor.Api.Observer.Commands
{
    public class NewObserverCommand : IRequest<int>
    {
        public int NgoId { get; set; }
        public string Phone { get; set; }
        public string Pin { get; set; }
        public string Name { get; set; }
        public bool SendSMS { get; set; }
    }
}
