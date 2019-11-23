using MediatR;

namespace VoteMonitor.Api.Auth.Commands
{
    public class RegisterDeviceId : IRequest<int>
    {
        public string MobileDeviceId { get; set; }
        public int ObserverId { get; set; }
    }
}
