using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VotingIrregularities.Api.Models.PollingStation {
    public class AddPollingStationInfo {
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
