using MediatR;

namespace VoteMonitor.Api.Notification.Commands;

public class SendNotificationCommand : IRequest<int>
{
    public string Channel { get; set; }
    public string From { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public List<int> Recipients { get; set; }
    public int SenderAdminId { get; set; }
}