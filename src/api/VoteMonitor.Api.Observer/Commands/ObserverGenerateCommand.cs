using MediatR;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Commands;

public record ObserverGenerateCommand(int NumberOfObservers, int NgoId) : IRequest<List<GeneratedObserver>>;
