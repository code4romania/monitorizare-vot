using System;
using MediatR;

namespace VoteMonitor.Api.Location.Commands
{
    public class UpdatePollingSectionCommand : IRequest<int>
    {
        public int IdObserver { get; set; }
        public int IdPollingStation { get; set; }
        public DateTime ObserverLeaveTime { get; set; }
    }
}
