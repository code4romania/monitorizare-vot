namespace VoteMonitor.Entities;

public class AnswerCorrupted
{
    public int IdObserver { get; set; }
    public int IdOptionToQuestion { get; set; }
    public DateTime LastModified { get; set; }
    public string Value { get; set; }
    public string CountyCode { get; set; }
    public string MunicipalityCode { get; set; }
    public int PollingStationNumber { get; set; }

    public virtual Observer Observer { get; set; }
    public virtual OptionToQuestion OptionAnswered { get; set; }
}
