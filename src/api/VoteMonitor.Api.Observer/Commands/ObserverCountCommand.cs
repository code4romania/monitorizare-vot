using MediatR;

namespace VoteMonitor.Api.Observer.Commands
{
    public class ObserverCountCommand : IRequest<int>
    {
        public int IdNgo { get; }
        public bool IsCallerOrganizer { get; }

        public ObserverCountCommand(int idNgo, bool isCallerOrganizer)
        {
            IdNgo = idNgo;
            IsCallerOrganizer = isCallerOrganizer;
        }
    }
}
