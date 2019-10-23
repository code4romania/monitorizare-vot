using System;
using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Location.Models
{
    public class UpdatePollingStationInfo {
        [Required(AllowEmptyStrings = false)]
        public string CountyCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int IdPollingStation { get; set; }

        [Required(AllowEmptyStrings = false)]
        public DateTime? ObserverLeaveTime { get; set; }
    }
}