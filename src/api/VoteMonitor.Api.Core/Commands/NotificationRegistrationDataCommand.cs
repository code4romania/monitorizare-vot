using MediatR;

namespace VoteMonitor.Api.Core.Commands;

public record NotificationRegistrationDataCommand(int ObserverId, string ChannelName, string Token) : IRequest<int>;
