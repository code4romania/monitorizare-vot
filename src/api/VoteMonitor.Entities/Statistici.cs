
namespace VoteMonitor.Entities;

public class SimpleStatistics
{
    public string Label { get; set; }
    public int Value { get; set; }
}

public class ComposedStatistics
{
    public string Label { get; set; }
    public int Code { get; set; }
    public int Value { get; set; }
}

public class OptionsStatistics
{
    public string Label { get; set; }
    public int Value { get; set; }
    public int Code { get; set; }
    public bool Flagged { get; set; }
}
