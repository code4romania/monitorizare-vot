using MediatR;

namespace VoteMonitor.Api.Observer.Commands;

public record ObserverCountCommand(int NgoId, bool IsCallerOrganizer) : IRequest<int>;
