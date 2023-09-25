using MediatR;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Queries;

public record GetObserverDetails(int NgoId, int ObserverId) : IRequest<ObserverModel>;
