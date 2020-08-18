
namespace VoteMonitor.Entities
{
    public partial class SimpleStatistics
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }

    public partial class ComposedStatistics
    {
        public string Label { get; set; }
        public int Code { get; set; }
        public int Value { get; set; }
    }

    public partial class OptionsStatistics
    {
        public string Label { get; set; }
        public int Value { get; set; }
        public int Code { get; set; }
        public bool Flagged { get; set; }
    }
}
