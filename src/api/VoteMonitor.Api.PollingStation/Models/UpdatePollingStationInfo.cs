using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.PollingStation.Models;

public class EditPollingStationInfo
{
     
    [Required(AllowEmptyStrings = false)]
    public DateTime? ObserverLeaveTime { get; set; }
}