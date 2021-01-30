using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Answer.Models;
using VoteMonitor.Api.Answer.Queries;
using VoteMonitor.Api.Core;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Answer.Handlers
{
    public class AnswersQueryHandler :
    IRequestHandler<AnswersQuery, ApiListResponse<AnswerQueryDto>>,
    IRequestHandler<FilledInAnswersQuery, List<QuestionDto<FilledInAnswerDto>>>,
    IRequestHandler<FormAnswersQuery, PollingStationInfoDto>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;

        public AnswersQueryHandler(VoteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiListResponse<AnswerQueryDto>> Handle(AnswersQuery message, CancellationToken cancellationToken)
        {
            var query = _context.Answers.Where(a => a.OptionAnswered.Flagged == message.Urgent);

            // Filter by the ngo id if the user is not of type organizer
            if (!message.Organizer)
            {
                query = query.Where(a => a.Observer.IdNgo == message.NgoId);
            }

            if (!string.IsNullOrEmpty(message.County))
            {
                query = query.Where(a => a.CountyCode == message.County);
            }

            if (message.PollingStationNumber > 0)
            {
                query = query.Where(a => a.PollingStationNumber == message.PollingStationNumber);
            }

            if (message.ObserverId > 0)
            {
                query = query.Where(a => a.IdObserver == message.ObserverId);
            }

            // Filter by observer phone number if specified
            if (!string.IsNullOrEmpty(message.ObserverPhoneNumber))
            {
                query = query.Where(a => a.Observer.Phone.Contains(message.ObserverPhoneNumber));
            }

            var answerQueryInfosQuery = query.GroupBy(a => new { a.IdPollingStation, a.CountyCode, a.PollingStationNumber, a.IdObserver, ObserverPhoneNumber = a.Observer.Phone, ObserverName = a.Observer.Name })
                .Select(x => new VoteMonitorContext.AnswerQueryInfo
                {
                    IdObserver = x.Key.IdObserver,
                    IdPollingStation = x.Key.IdPollingStation,
                    PollingStation = x.Key.CountyCode + " " + x.Key.PollingStationNumber.ToString(),
                    ObserverName = x.Key.ObserverName,
                    ObserverPhoneNumber = x.Key.ObserverPhoneNumber,
                    LastModified = x.Max(a => a.LastModified)
                });

            var count = await answerQueryInfosQuery.CountAsync(cancellationToken: cancellationToken);

            var pagedAnswerQueryInfo = await answerQueryInfosQuery
                .OrderByDescending(aqi => aqi.LastModified)
                .Skip((message.Page - 1) * message.PageSize)
                .Take(message.PageSize)
                .ToListAsync(cancellationToken: cancellationToken);

            return new ApiListResponse<AnswerQueryDto>
            {
                Data = pagedAnswerQueryInfo.Select(x => _mapper.Map<AnswerQueryDto>(x)).ToList(),
                Page = message.Page,
                PageSize = message.PageSize,
                TotalItems = count
            };
        }
        
        public async Task<List<QuestionDto<FilledInAnswerDto>>> Handle(FilledInAnswersQuery message, CancellationToken cancellationToken)
        {
            var answers = await _context.Answers
                .Include(r => r.OptionAnswered)
                    .ThenInclude(rd => rd.Question)
                .Include(r => r.OptionAnswered)
                    .ThenInclude(rd => rd.Option)
                .Where(r => r.IdObserver == message.ObserverId && r.IdPollingStation == message.PollingStationId)
                .ToListAsync(cancellationToken: cancellationToken);

            var questions = answers
                .Select(r => r.OptionAnswered.Question)
                .ToList();

            return questions.Select(i => _mapper.Map<QuestionDto<FilledInAnswerDto>>(i)).ToList();
        }

        public async Task<PollingStationInfoDto> Handle(FormAnswersQuery message, CancellationToken cancellationToken)
        {
            var pollingStationInfo = await _context.PollingStationInfos
                .FirstOrDefaultAsync(rd => rd.IdObserver == message.ObserverId
                && rd.IdPollingStation == message.PollingStationId, cancellationToken: cancellationToken);

            return _mapper.Map<PollingStationInfoDto>(pollingStationInfo);
        }
    }
}
