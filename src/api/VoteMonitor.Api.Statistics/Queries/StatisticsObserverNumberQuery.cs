using MediatR;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Statistics.Models;

namespace VoteMonitor.Api.Statistics.Queries
{
    public class StatisticsObserverNumberQuery : StatisticsPaginatedQuery, IRequest<ApiListResponse<SimpleStatisticsModel>>
    {
    }
}