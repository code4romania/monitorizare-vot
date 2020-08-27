using System;
using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.PollingStation.Models
{
    public class CreatePollingStationInfo
    {
        [Required(AllowEmptyStrings = false)]
        public int IdPollingStation { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CountyCode { get; set; }

        public bool? UrbanArea { get; set; }
        public DateTime? ObserverLeaveTime { get; set; }
        public DateTime? ObserverArrivalTime { get; set; }
        public bool? IsPollingStationPresidentFemale { get; set; }
    }
}