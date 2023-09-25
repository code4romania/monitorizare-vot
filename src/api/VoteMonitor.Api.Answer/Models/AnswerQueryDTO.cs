namespace VoteMonitor.Api.Answer.Models;

public class AnswerQueryDto
{
    public int ObserverId { get; set; }
    public int PollingStationId { get; set; }
    public string ObserverName { get; set; }
    public string ObserverPhoneNumber { get; set; }
    public string PollingStationName { get; set; }
    public DateTime LastModified { get; set; }
}
