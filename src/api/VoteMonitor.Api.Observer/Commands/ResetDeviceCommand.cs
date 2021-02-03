using MediatR;

namespace VoteMonitor.Api.Observer.Commands
{
    public class ResetDeviceCommand : IRequest<int>
    {
        public int NgoId { get; set; }
        public string PhoneNumber { get; set; }

        public bool Organizer { get; set; }
    }
}