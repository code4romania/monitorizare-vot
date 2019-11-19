using MediatR;

namespace VoteMonitor.Api.Observer.Commands
{
    public class ResetPasswordCommand : IRequest<bool>
    {
        public ResetPasswordCommand(int id, string phone, string pin)
        {
            IdNgo = id;
            PhoneNumber = phone;
            Pin = pin;

        }
        public int IdNgo { get; }
        public string PhoneNumber { get; }
        public string Pin { get; }
    }
}