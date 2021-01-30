using System;
using MediatR;

namespace VoteMonitor.Api.PollingStation.Queries
{
    public class CreatePollingStationInfo : IRequest
    {
        public int ObserverId { get; set; }
        public int PollingStationId { get; set; }
        public string CountyCode { get; set; }
        public bool? UrbanArea { get; set; }
        public DateTime? ObserverLeaveTime { get; set; }
        public DateTime? ObserverArrivalTime { get; set; }
        public bool? IsPollingStationPresidentFemale { get; set; }
    }
}