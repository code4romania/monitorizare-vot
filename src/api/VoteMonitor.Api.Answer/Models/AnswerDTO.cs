namespace VoteMonitor.Api.Answer.Models;

public class AnswerDto
{
    public int QuestionId { get; set; }
    public int PollingStationId { get; set; }
    public string CountyCode { get; set; }
    public int PollingStationNumber { get; set; }
    public List<SelectedOptionDto> Options { get; set; }
}
