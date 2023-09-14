namespace VoteMonitor.Api.Statistics.Queries;

public record StatisticsQuery(int NgoId, bool IsOrganizer, int CacheHours, int CacheMinutes, int CacheSeconds);
