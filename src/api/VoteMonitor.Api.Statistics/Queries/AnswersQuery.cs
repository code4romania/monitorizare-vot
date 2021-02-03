using MediatR;
using VoteMonitor.Api.Statistics.Models;

namespace VoteMonitor.Api.Statistics.Queries
{
    public class AnswersQuery : IRequest<SimpleStatisticsModel>
    {
    }
}