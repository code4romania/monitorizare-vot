using MediatR;
using System;

namespace VoteMonitor.Api.PollingStation.Queries
{
    public class UpdatePollingStationInfo : IRequest
    {
        public int ObserverId { get; set; }
        public int PollingStationId { get; set; }
        public DateTime ObserverLeaveTime { get; set; }
    }
}
