using MediatR;
using VoteMonitor.Api.Statistics.Models;

namespace VoteMonitor.Api.Statistics.Queries;

public record StatisticsOptionsQuery(int QuestionId, int NgoId, bool IsOrganizer, int CacheHours, int CacheMinutes, int CacheSeconds) : StatisticsQuery(NgoId, IsOrganizer, CacheHours, CacheMinutes, CacheSeconds), IRequest<StatisticsOptionsModel>;
