namespace VoteMonitor.Api.Form.Models.Options;

public class OptionDTO
{
    public int Id { get; set; }
    public bool IsFreeText { get; set; }
    public string Text { get; set; }
    public string Hint { get; set; }
}