using MediatR;

namespace VoteMonitor.Api.Notification.Commands;

public record SendNotificationToAllCommand(int SenderAdminId, string Channel, string From, string Title, string Message) : IRequest<int>;
