namespace VoteMonitor.Api.Statistics.Queries
{
    public class StatisticsQuery
    {
        public int IdONG { get; set; }
        public bool Organizator { get; set; }
        public int CacheHours { get; set; }
        public int CacheMinutes { get; set; }
        public int CacheSeconds { get; set; }
    }
}
