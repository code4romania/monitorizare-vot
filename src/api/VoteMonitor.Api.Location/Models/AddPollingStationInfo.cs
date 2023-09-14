using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Location.Models;

public class AddPollingStationInfo
{
    [Required(AllowEmptyStrings = false)]
    public int PollingStationNumber { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string CountyCode { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string MunicipalityCode { get; set; }

    public DateTime? ObserverLeaveTime { get; set; }
    public DateTime? ObserverArrivalTime { get; set; }
    public bool? IsPollingStationPresidentFemale { get; set; }
}
