using System;
using MediatR;

namespace VoteMonitor.Api.PollingStation.Queries
{
    public class CreatePollingStationInfo : IRequest
    {
        public int IdObserver { get; set; }
        public int IdPollingStation { get; set; }
        public string CountyCode { get; set; }
        public bool? UrbanArea { get; set; }
        public DateTime? ObserverLeaveTime { get; set; }
        public DateTime? ObserverArrivalTime { get; set; }
        public bool? IsPollingStationPresidentFemale { get; set; }
    }
}