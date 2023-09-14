using MediatR;

namespace VoteMonitor.Api.Notification.Commands;

public record SendNotificationCommand(List<int> Recipients, int SenderAdminId, string Channel, string From, string Title, string Message) : IRequest<int>;
