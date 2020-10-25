using MediatR;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Queries
{
    public class GetObserverDetails : IRequest<ObserverModel>
    {
        public int ObserverId { get; }
        public int NgoId { get; }
        public GetObserverDetails(int ngoId, int observerId)
        {
            NgoId = ngoId;
            ObserverId = observerId;
        }
    }
}