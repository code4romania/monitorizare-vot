using MediatR;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Statistics.Queries;

namespace VoteMonitor.Api.Statistics.Models
{
    public class SimpleStatisticsModel
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class StatisticsObserversNumberQuery : StatisticsPaginatedQuery, IRequest<ApiListResponse<SimpleStatisticsModel>>
    {
    }

    public class StatisticiTopSesizariQuery : StatisticsPaginatedQuery, IRequest<ApiListResponse<SimpleStatisticsModel>>
    {
        public string Formular { get; set; }
        public StatisticsGroupingTypes Grupare { get; set; }
    }
}
