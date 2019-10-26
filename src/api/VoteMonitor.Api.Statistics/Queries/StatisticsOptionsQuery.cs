using MediatR;
using VoteMonitor.Api.Statistics.Models;

namespace VoteMonitor.Api.Statistics.Queries {
    public class StatisticsOptionsQuery : StatisticsQuery, IRequest<StatisticsOptionsModel>
    {
        public int QuestionId { get; set; }
    }
}
