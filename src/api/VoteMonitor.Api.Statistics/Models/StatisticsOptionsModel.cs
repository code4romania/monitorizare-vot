using System.Collections.Generic;

namespace VoteMonitor.Api.Statistics.Models
{
    public class StatisticsOptionsModel
    {
        public int QuestionId { get; set; }
        public IList<OptiuniStatisticsModel> Options { get; set; }
        public int Total { get; set; }
    }

    public class OptiuniStatisticsModel : SimpleStatisticsModel
    {
        public int OptionId { get; set; }
        public bool Flagged { get; set; }
    }
}
