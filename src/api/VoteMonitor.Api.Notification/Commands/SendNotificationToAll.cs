using MediatR;

namespace VoteMonitor.Api.Notification.Commands
{
	public class SendNotificationToAll : IRequest<int>
	{
		public string Channel { get; }
		public string From { get; }
		public string Title { get; }
		public string Message { get; }

		public SendNotificationToAll(string channel, string sender, string title, string message)
		{
			Channel = channel;
			From = sender;
			Title = title;
			Message = message;
		}
	}
}