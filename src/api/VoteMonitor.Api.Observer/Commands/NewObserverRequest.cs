using MediatR;

namespace VoteMonitor.Api.Observer.Commands
{
    public class NewObserverRequest : IRequest<int>
    {
        public int IdOng { get; set; }
        public string NumarTelefon { get; set; }
        public string PIN { get; set; }
        public string Nume { get; set; }
        public bool SendSMS { get; set; }
    }
}
