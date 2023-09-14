using MediatR;

namespace VoteMonitor.Api.Location.Commands;

public class RegisterPollingStationCommand : IRequest<int>
{
    public int IdObserver { get; set; }
    public int PollingStationNumber { get; set; }
    public string CountyCode { get; set; }
    public string MunicipalityCode { get; set; }
    public DateTime? ObserverLeaveTime { get; set; }
    public DateTime? ObserverArrivalTime { get; set; }
    public bool? IsPollingStationPresidentFemale { get; set; }
}
