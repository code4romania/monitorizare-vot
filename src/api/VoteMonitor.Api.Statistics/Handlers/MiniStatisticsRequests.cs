using MediatR;
using VoteMonitor.Api.Statistics.Models;

namespace VoteMonitor.Api.Statistics.Handlers
{
    public class AnswersRequest : IRequest<SimpleStatisticsModel>
    {
    }
    public class StationsVisitedRequest : IRequest<SimpleStatisticsModel>
    {
    }
    public class FlaggedAnswersRequest : IRequest<SimpleStatisticsModel>
    {
    }
    public class CountiesVisitedRequest : IRequest<SimpleStatisticsModel>
    {
    }
    public class NotesUploadedRequest : IRequest<SimpleStatisticsModel>
    {
    }
    public class LoggedInObserversRequest : IRequest<SimpleStatisticsModel>
    {
    }
}
