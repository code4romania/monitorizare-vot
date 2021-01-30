using MediatR;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Statistics.Models;

namespace VoteMonitor.Api.Statistics.Queries
{
    public class StatisticsTopIrregularitiesQuery : StatisticsPaginatedQuery, IRequest<ApiListResponse<SimpleStatisticsModel>>
    {
        public string FormCode { get; set; }
        public StatisticsGroupingTypes GroupType { get; set; }
    }
}