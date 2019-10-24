using MediatR;

namespace VoteMonitor.Api.Notification.Commands
{
    public class NotificationRegDataCommand : IRequest<int>
    {
        public int ObserverId { get; set; }
        public string ChannelName { get; set; }
        public string Token { get; set; }
    }
}