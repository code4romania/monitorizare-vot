using VoteMonitor.Api.Core;

namespace VoteMonitor.Api.Statistics.Queries
{

    public class StatisticsPaginatedQuery : PagingModel
    {
        public int NgoId { get; set; }
        public bool IsOrganizer { get; set; }
        public int CacheHours { get; set; }
        public int CacheMinutes { get; set; }
        public int CacheSeconds { get; set; }
    }
}
