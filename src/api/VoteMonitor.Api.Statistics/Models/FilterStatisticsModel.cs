using VoteMonitor.Api.Core;

namespace VoteMonitor.Api.Statistics.Models
{
    public class FilterStatisticsModel : PagingModel
    {
        public string FormCode { get; set; }
        public StatisticsGroupingTypes GroupingType { get; set; }
    }
}
