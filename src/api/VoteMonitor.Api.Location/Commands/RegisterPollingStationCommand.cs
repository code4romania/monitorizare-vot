using MediatR;

namespace VoteMonitor.Api.Location.Commands;

public class RegisterPollingStationCommand : IRequest<int>
{
    public int IdObserver { get; set; }
    public int PollingStationNumber { get; set; }
    public string CountyCode { get; set; }
    public string MunicipalityCode { get; set; }
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
