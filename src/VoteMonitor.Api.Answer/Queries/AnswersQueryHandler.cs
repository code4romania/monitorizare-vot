using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Answer.Models;
using VoteMonitor.Api.Core;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Answer.Queries {
    public class AnswersQueryHandler :
    IRequestHandler<AnswersQuery, ApiListResponse<AnswerQueryDTO>>,
    IRequestHandler<FilledInAnswersQuery, List<QuestionDTO<FilledInAnswerDTO>>>,
    IRequestHandler<FormAnswersQuery, PollingStationInfosDTO> {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;

        public AnswersQueryHandler(VoteMonitorContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiListResponse<AnswerQueryDTO>> Handle(AnswersQuery message, CancellationToken cancellationToken) {
            //var query = from a in _context.Answers
            //            join o in _context.Observers on a.IdObserver equals o.Id
            //            join oq in _context.OptionsToQuestions on o.Id equals oq.IdQuestion
            //            group a by new { a.IdPollingStation, a.CountyCode, a.IdObserver } into g
            //            where oq.Flagged == message.Urgent
            //            && o.IdNgo == message.Organizer ? message.IdONG : o.IdNgo
            //            && a.CountyCode == string.IsNullOrEmpty(message.County) ? a.CountyCode : message.County
            //            && a.PollingStationNumber == message.PollingStationNumber > 0 ? message.PollingStationNumber : a.PollingStationNumber
            //            && a.IdObserver == message.ObserverId > 0 ? message.ObserverId : a.IdObserver

            //            select g.Key

            //            ;
            
            var queryUnPaged = $@"SELECT IdPollingStation, A.IdObserver, O.Name as ObserverName, CONCAT(CountyCode, ' ', PollingStationNumber) AS PollingStation, MAX(LastModified) AS LastModified
                FROM Answers A
                INNER JOIN Observers O ON O.Id = A.IdObserver
                INNER JOIN OptionsToQuestions OQ ON OQ.Id = A.IdOptionToQuestion
                WHERE OQ.Flagged = {Convert.ToInt32(message.Urgent)}";

            // Filter by the organizer flag if specified
            if (!message.Organizer)
                queryUnPaged = $"{queryUnPaged} AND O.IdNgo = {message.IdONG}";

            // Filter by county if specified
            if (!string.IsNullOrEmpty(message.County))
                queryUnPaged = $"{queryUnPaged} AND A.CountyCode = '{message.County}'";

            // Filter by polling station if specified
            if (message.PollingStationNumber > 0)
                queryUnPaged = $"{queryUnPaged} AND A.PollingStationNumber = {message.PollingStationNumber}";

            // Filter by polling station if specified
            if (message.ObserverId > 0)
                queryUnPaged = $"{queryUnPaged} AND A.IdObserver = {message.ObserverId}";

            queryUnPaged = $"{queryUnPaged} GROUP BY IdPollingStation, CountyCode, PollingStationNumber, A.IdObserver, O.Name, CountyCode";

            var queryPaged = $@"{queryUnPaged} ORDER BY MAX(LastModified) DESC OFFSET {(message.Page - 1) * message.PageSize} ROWS FETCH NEXT {message.PageSize} ROWS ONLY";

            var sectiiCuObservatoriPaginat = await _context.AnswerQueryInfos
                .FromSql(queryPaged)
                .ToListAsync(cancellationToken: cancellationToken);

            var count = await _context.AnswerQueryInfos
                .FromSql(queryUnPaged)
                .CountAsync(cancellationToken: cancellationToken);

            return new ApiListResponse<AnswerQueryDTO> {
                Data = sectiiCuObservatoriPaginat.Select(x => _mapper.Map<AnswerQueryDTO>(x)).ToList(),
                Page = message.Page,
                PageSize = message.PageSize,
                TotalItems = count
            };
        }


        public async Task<List<QuestionDTO<FilledInAnswerDTO>>> Handle(FilledInAnswersQuery message, CancellationToken cancellationToken) {
            var raspunsuri = await _context.Answers
                .Include(r => r.OptionAnswered)
                    .ThenInclude(rd => rd.Question)
                .Include(r => r.OptionAnswered)
                    .ThenInclude(rd => rd.Option)
                .Where(r => r.IdObserver == message.ObserverId && r.IdPollingStation == message.PollingStationId)
                .ToListAsync();

            var intrebari = raspunsuri
                .Select(r => r.OptionAnswered.Question)
                .ToList();

            return intrebari.Select(i => _mapper.Map<QuestionDTO<FilledInAnswerDTO>>(i)).ToList();
        }

        public async Task<PollingStationInfosDTO> Handle(FormAnswersQuery message, CancellationToken cancellationToken) {
            var raspunsuriFormular = await _context.PollingStationInfos
                .FirstOrDefaultAsync(rd => rd.IdObserver == message.ObserverId 
                && rd.IdPollingStation == message.PollingStationId);

            return _mapper.Map<PollingStationInfosDTO>(raspunsuriFormular);
        }
    }
}
