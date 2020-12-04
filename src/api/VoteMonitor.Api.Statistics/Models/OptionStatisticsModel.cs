namespace VoteMonitor.Api.Statistics.Models
{
    public class OptionStatisticsModel : SimpleStatisticsModel
    {
        public int OptionId { get; set; }
        public bool IsFlagged { get; set; }
    }
}