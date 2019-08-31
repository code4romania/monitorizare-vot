using MediatR;

namespace VoteMonitor.Api.Observer.Commands
{
    public class ResetDeviceCommand : IRequest<int>
    {
        public ResetDeviceCommand(int id, string phone)
        {
            IdNgo = id;
            PhoneNumber = phone;
        }

        public int IdNgo;
        public string PhoneNumber;
    }
}