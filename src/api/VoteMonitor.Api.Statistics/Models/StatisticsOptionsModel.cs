using System.Collections.Generic;

namespace VoteMonitor.Api.Statistics.Models
{
    public class StatisticsOptionsModel
    {
        public int QuestionId { get; set; }
        public IList<OptionStatisticsModel> Options { get; set; }
        public int Total { get; set; }
    }
}
