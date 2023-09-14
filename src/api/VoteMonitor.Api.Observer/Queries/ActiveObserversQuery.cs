using MediatR;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Queries;

public record ActiveObserversQuery(int NgoId, string[] CountyCodes, bool CurrentlyCheckedIn) : IRequest<List<ObserverModel>>;
