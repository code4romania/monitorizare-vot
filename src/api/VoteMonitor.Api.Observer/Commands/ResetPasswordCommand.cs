using MediatR;

namespace VoteMonitor.Api.Observer.Commands
{
    public class ResetPasswordCommand : IRequest<string>
    {
        public ResetPasswordCommand(int id, string phone)
        {
            IdNgo = id;
            PhoneNumber = phone;
        }
        public int IdNgo;
        public string PhoneNumber;
    }
}