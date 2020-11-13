using MediatR;

namespace VoteMonitor.Api.Notification.Commands
{
    public class SendNotificationToAll : IRequest<int>
    {
        public string Channel { get; }
        public string From { get; }
        public string Title { get; }
        public string Message { get; }
        public int SenderAdminId { get; }

        public SendNotificationToAll(int senderAdminId, string channel, string sender, string title, string message)
        {
            SenderAdminId = senderAdminId;
            Channel = channel;
            From = sender;
            Title = title;
            Message = message;
        }
    }
}