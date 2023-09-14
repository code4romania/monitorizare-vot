using MediatR;

namespace VoteMonitor.Api.Observer.Commands;

public record DeleteObserverCommand(int ObserverId) : IRequest<bool>;
