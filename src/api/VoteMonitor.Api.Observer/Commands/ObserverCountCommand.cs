using MediatR;

namespace VoteMonitor.Api.Observer.Commands
{
    public class ObserverCountCommand : IRequest<int>
    {
        public int NgoId { get; }
        public bool IsCallerOrganizer { get; }

        public ObserverCountCommand(int ngoId, bool isCallerOrganizer)
        {
            NgoId = ngoId;
            IsCallerOrganizer = isCallerOrganizer;
        }
    }
}
