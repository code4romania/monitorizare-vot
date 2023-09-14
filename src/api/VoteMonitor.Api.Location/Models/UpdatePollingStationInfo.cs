using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Location.Models;

public class UpdatePollingStationInfo
{
    [Required(AllowEmptyStrings = false)]
    public string CountyCode { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string MunicipalityCode { get; set; }

    [Required(AllowEmptyStrings = false)]
    public int PollingStationNumber { get; set; }

    [Required(AllowEmptyStrings = false)]
    public DateTime? ObserverLeaveTime { get; set; }
}
