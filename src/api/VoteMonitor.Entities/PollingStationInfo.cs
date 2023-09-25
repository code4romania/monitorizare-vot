namespace VoteMonitor.Entities;

public class PollingStationInfo
{
    public int IdObserver { get; set; }
    public int IdPollingStation { get; set; }
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

    public virtual Observer Observer { get; set; }
    public virtual PollingStation PollingStation { get; set; }
}
