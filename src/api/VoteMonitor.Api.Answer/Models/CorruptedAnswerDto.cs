namespace VoteMonitor.Api.Answer.Models;

public class CorruptedAnswerDto
{
    public int QuestionId { get; set; }
    public string CountyCode { get; set; }
    public string MunicipalityCode { get; set; }
    public int PollingStationNumber { get; set; }
    public List<SelectedOptionDto> Options { get; set; }
}
