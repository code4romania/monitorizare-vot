using MediatR;

namespace VoteMonitor.Api.Observer.Commands;

public record RemoveDeviceIdCommand(int ObserverId) : IRequest;
