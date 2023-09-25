namespace VoteMonitor.Api.Answer.Models;

public class PollingStationInfoDto
{
    public DateTime LastModified { get; set; }
    public DateTime? ObserverArrivalTime { get; set; }
    public DateTime? ObserverLeaveTime { get; set; }

    public int NumberOfVotersOnTheList { get; set; }

    public int NumberOfCommissionMembers { get; set; }

    public int NumberOfFemaleMembers { get; set; }

    public int MinPresentMembers { get; set; }

    public bool ChairmanPresence { get; set; }

    public bool SinglePollingStationOrCommission { get; set; }

    public bool AdequatePollingStationSize { get; set; }
}
