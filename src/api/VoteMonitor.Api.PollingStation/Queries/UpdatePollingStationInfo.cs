using MediatR;
using System;

namespace VoteMonitor.Api.PollingStation.Queries
{
    public class UpdatePollingStationInfo : IRequest
    {
        public int IdObserver { get; set; }
        public int IdPollingStation { get; set; }
        public DateTime ObserverLeaveTime { get; set; }
    }
}
