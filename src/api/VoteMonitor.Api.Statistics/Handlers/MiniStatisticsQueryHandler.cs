using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Statistics.Models;
using VoteMonitor.Api.Statistics.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Statistics.Handlers
{
    public class MiniStatisticsQueryHandler :
        IRequestHandler<AnswersQuery, SimpleStatisticsModel>,
        IRequestHandler<StationsVisitedQuery, SimpleStatisticsModel>,
        IRequestHandler<FlaggedAnswersQuery, SimpleStatisticsModel>,
        IRequestHandler<CountiesVisitedQuery, SimpleStatisticsModel>,
        IRequestHandler<NotesUploadedQuery, SimpleStatisticsModel>,
        IRequestHandler<LoggedInObserversQuery, SimpleStatisticsModel>
    {
        private readonly VoteMonitorContext _context;

        public MiniStatisticsQueryHandler(VoteMonitorContext context)
        {
            _context = context;
        }

        public async Task<SimpleStatisticsModel> Handle(AnswersQuery message, CancellationToken token)
        {
            var number = await _context.Answers
                .Include(a => a.Observer)
                .Where(x => x.Observer.IdNgo != 1)
                .Where(x => x.Observer.Ngo.IsActive)
                .Where(x => !x.Observer.IsTestObserver)
                .CountAsync(token);

            return new SimpleStatisticsModel
            {
                Label = "Number of answers submitted",
                Value = number.ToString()
            };
        }
        public async Task<SimpleStatisticsModel> Handle(StationsVisitedQuery message, CancellationToken token)
        {
            var number = await _context.Answers
                .Include(a => a.Observer)
                .Where(x => x.Observer.IdNgo != 1)
                .Where(x => x.Observer.Ngo.IsActive)
                .Where(x => !x.Observer.IsTestObserver)
                .Select(r => r.IdPollingStation)
                .Distinct()
                .CountAsync(token);

            return new SimpleStatisticsModel
            {
                Label = "Number of Polling Stations visited",
                Value = number.ToString()
            };
        }
        public async Task<SimpleStatisticsModel> Handle(CountiesVisitedQuery message, CancellationToken token)
        {
            var number = await _context.Answers
                .Include(a => a.Observer)
                .Where(x => x.Observer.IdNgo != 1)
                .Where(x => x.Observer.Ngo.IsActive)
                .Where(x => !x.Observer.IsTestObserver)
                .Select(r => r.CountyCode)
                .Distinct()
                .CountAsync(token);

            return new SimpleStatisticsModel
            {
                Label = "Number of Counties visited",
                Value = number.ToString()
            };
        }
        public async Task<SimpleStatisticsModel> Handle(NotesUploadedQuery message, CancellationToken token)
        {
            var number = await _context.Notes
                .Include(n => n.Observer)
                .Where(x => x.Observer.IdNgo != 1)
                .Where(x => x.Observer.Ngo.IsActive)
                .Where(x => !x.Observer.IsTestObserver)
                .CountAsync(token);

            return new SimpleStatisticsModel
            {
                Label = "Number of notes submitted",
                Value = number.ToString()
            };
        }
        public async Task<SimpleStatisticsModel> Handle(LoggedInObserversQuery message, CancellationToken token)
        {
            var number = await _context.PollingStationInfos
                .Include(a => a.Observer)
                .Where(x => x.Observer.IdNgo != 1)
                .Where(x => x.Observer.Ngo.IsActive)
                .Where(x => !x.Observer.IsTestObserver)
                .Select(pi => pi.IdObserver)
                .Distinct()
                .CountAsync(token);

            return new SimpleStatisticsModel
            {
                Label = "Number of logged in Observers",
                Value = number.ToString()
            };
        }
        public async Task<SimpleStatisticsModel> Handle(FlaggedAnswersQuery message, CancellationToken token)
        {
            var number = await _context.Answers
                .Include(a => a.Observer)
                .Where(x => x.Observer.IdNgo != 1)
                .Where(x => x.Observer.Ngo.IsActive)
                .Where(x => !x.Observer.IsTestObserver)
                .Include(r => r.OptionAnswered)
                .CountAsync(r => r.OptionAnswered.Flagged, token);

            return new SimpleStatisticsModel
            {
                Label = "Number of flagged answers submitted",
                Value = number.ToString()
            };
        }
    }
}
