using MediatR;
using System;

namespace VotingIrregularities.Domain.SectieAggregate
{
    public class UpdatePollingSectionCommand : IRequest<int>
    {
        public int IdObserver { get; set; }
        public int IdPollingStation { get; set; }
        public DateTime ObserverLeaveTime { get; set; }
    }
}
