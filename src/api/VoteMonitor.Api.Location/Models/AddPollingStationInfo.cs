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

    public DateTime? ObserverArrivalTime { get; set; }
    
    [Required]
    public int? NumberOfVotersOnTheList { get; set; }

    [Required]
    public int? NumberOfCommissionMembers { get; set; }

    [Required]
    public int? NumberOfFemaleMembers { get; set; }

    [Required]
    public int? MinPresentMembers { get; set; }

    [Required]
    public bool? ChairmanPresence { get; set; }

    public bool? SinglePollingStationOrCommission { get; set; }

    public bool? AdequatePollingStationSize { get; set; }
}
