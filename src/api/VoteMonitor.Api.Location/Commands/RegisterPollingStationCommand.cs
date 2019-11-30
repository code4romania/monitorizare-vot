using MediatR;
using System;

namespace VoteMonitor.Api.Location.Commands
{
    public class RegisterPollingStationCommand : IRequest<int>
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