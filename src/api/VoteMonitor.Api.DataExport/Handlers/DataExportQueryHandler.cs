using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.DataExport.Models;
using VoteMonitor.Api.DataExport.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.DataExport.Handlers
{
    public class DataExportQueryHandler : IRequestHandler<GetDataForExport, IEnumerable<ExportModelDto>>,
         IRequestHandler<GetNotesForExport, IEnumerable<NotesExportModel>>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;

        public DataExportQueryHandler(VoteMonitorContext context, ILogger<DataExportQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<IEnumerable<ExportModelDto>> Handle(GetDataForExport request, CancellationToken cancellationToken)
        {
            var query = @" SELECT
			obs.Phone as [ObserverPhone],
			obs.IdNgo,
			f.Code as FormCode,
			q.Text as QuestionText,
			o.Text as [OptionText],
			a.[Value] as [AnswerFreeText],
			a.LastModified,
			a.CountyCode,
			a.PollingStationNumber,
			count(n.Text) as NumberOfNotes,
			count(na.Path) as NumberOfAttachments
		FROM 
			(Answers a 
			INNER JOIN Observers obs
				ON a.IdObserver = obs.Id
			INNER JOIN OptionsToQuestions oq
				ON a.IdOptionToQuestion = oq.Id
			INNER JOIN Options o 
				ON oq.IdOption = o.Id
			INNER JOIN Questions q
				ON oq.IdQuestion = q.Id
			INNER JOIN FormSections fs
				ON q.IdSection = fs.Id
			INNER JOIN Forms f
				ON fs.IdForm = f.Id)
			LEFT JOIN Notes n
				ON n.IdQuestion = q.Id AND n.IdObserver = obs.Id AND n.IdPollingStation = a.IdPollingStation
			LEFT JOIN NotesAttachments na
				on na.NoteId = n.Id
		WHERE
			a.LastModified >= @from
                        AND obs.IsTestObserver = 0
            ";

            var parameters = new DynamicParameters();
            parameters.Add("from", request.From ?? new DateTime(2019, 11, 08, 6, 0, 0));

            if (request.ApplyFilters)
            {
                if (request.To.HasValue)
                {
                    query += " AND a.LastModified <= @to ";
                    parameters.Add("to", request.To ?? DateTime.Now.AddDays(2));
                }

                if (request.ObserverId.HasValue)
                {
                    query += " AND obs.Id = @ObserverId ";
                    parameters.Add("ObserverId", request.ObserverId);
                }

                if (request.NgoId.HasValue)
                {
                    query += " AND obs.IdNgo = @IdNgo ";
                    parameters.Add("IdNgo", request.NgoId);
                }

                if (!string.IsNullOrEmpty(request.County))
                {
                    query += " AND a.CountyCode = @County ";
                    parameters.Add("County", request.County);
                }

                if (request.PollingStationNumber.HasValue)
                {
                    query += " AND a.PollingStationNumber = @PollingStationNumber ";
                    parameters.Add("PollingStationNumber", request.PollingStationNumber);
                }
            }

            query = query + @"
                group by obs.Phone ,
                        obs.IdNgo,
                        f.Code,
                        q.Text,
                        o.Text,
                        a.[Value],
                        a.LastModified,
                        a.CountyCode,
                        a.PollingStationNumber";

            IEnumerable<ExportModelDto> data = Enumerable.Empty<ExportModelDto>();
            using (var db = _context.Database.GetDbConnection())
            {
                db.Open();
                data = db.Query<ExportModelDto>(sql: query.ToString(), param: parameters, commandTimeout: 60);
            }

            return Task.FromResult(data);
        } 
        
        public Task<IEnumerable<NotesExportModel>> Handle(GetNotesForExport request, CancellationToken cancellationToken)
        {
            var queryNotesNotAttachedToQuestions = @"
            SELECT obs.Phone AS [ObserverPhone],
                obs.IdNgo,
                '' AS FormCode,
                '' AS QuestionText,
                '' AS [OptionText],
                '' AS [AnswerFreeText],
                n.Text AS NoteText,
                na.Path AS [NoteAttachmentPath],
                NULL AS LastModified,
                cc.Code AS CountyCode,
                ps.Number AS PollingStationNumber
            FROM Notes n
            INNER JOIN Observers obs ON n.IdObserver = obs.Id
            LEFT JOIN NotesAttachments na ON na.NoteId = n.Id
            INNER JOIN PollingStations ps ON n.IdPollingStation = ps.Id
            INNER JOIN Counties cc ON ps.IdCounty = cc.Id
            WHERE obs.IsTestObserver = 0
            AND n.IdQuestion IS NULL
            AND n.LastModified >= @from
            AND obs.IsTestObserver = 0";

            var queryNotesForAnsweredQuestions = @"
            SELECT obs.Phone AS [ObserverPhone],
                   obs.IdNgo,
                   f.Code AS FormCode,
                   q.Text AS QuestionText,
                   o.Text AS [OptionText],
                   a.[Value] AS [AnswerFreeText],
                   n.Text AS NoteText,
                   na.Path AS [NoteAttachmentPath],
                   a.LastModified,
                   a.CountyCode,
                   a.PollingStationNumber
            FROM Notes n
            INNER JOIN Observers obs ON n.IdObserver = obs.Id
            INNER JOIN Questions q ON n.IdQuestion = q.Id
            INNER JOIN OptionsToQuestions oq ON oq.IdQuestion = q.Id
            INNER JOIN OPTIONS o ON oq.IdOption = o.Id
            INNER JOIN FormSections fs ON q.IdSection = fs.Id
            INNER JOIN Forms f ON fs.IdForm = f.Id
            INNER JOIN Answers a ON a.IdOptionToQuestion = oq.Id
            AND a.IdObserver = obs.Id
            AND n.IdPollingStation = a.IdPollingStation
            LEFT JOIN NotesAttachments na ON na.NoteId = n.Id
            WHERE obs.IsTestObserver = 0
            AND n.IdQuestion IS NOT NULL
            AND n.LastModified >= @from";

            var queryForNotesAttachedToQuestionsButNoAnswers = @"
            SELECT obs.Phone AS [ObserverPhone],
                   obs.IdNgo,
                   f.Code AS FormCode,
                   q.Text AS QuestionText,
                   '' AS [OptionText],
                   '' AS [AnswerFreeText],
                   n.Text AS NoteText,
                   na.Path AS [NoteAttachmentPath],
                   '' AS LastModified,
                   cc.Code AS CountyCode,
                   ps.Number AS PollingStationNumber
            FROM Notes n
            INNER JOIN Observers obs ON n.IdObserver = obs.Id
            INNER JOIN Questions q ON n.IdQuestion = q.Id
            INNER JOIN FormSections fs ON q.IdSection = fs.Id
            INNER JOIN Forms f ON fs.IdForm = f.Id
            LEFT JOIN NotesAttachments na ON na.NoteId = n.Id
            INNER JOIN PollingStations ps ON n.IdPollingStation = ps.Id
            INNER JOIN Counties cc ON ps.IdCounty = cc.Id
            WHERE NOT EXISTS
                (SELECT *
                 FROM Answers a
                 INNER JOIN OptionsToQuestions oq ON oq.Id = a.IdOptionToQuestion
                 AND oq.IdQuestion = q.Id
                 WHERE a.IdObserver = obs.id )
                    AND obs.IsTestObserver = 0
                 AND n.LastModified >= @from";

            var parameters = new DynamicParameters();
            parameters.Add("from", request.From ?? new DateTime(2019, 11, 08, 6, 0, 0));

            if (request.ApplyFilters)
            {
                if (request.To.HasValue)
                {
                    queryNotesNotAttachedToQuestions += " AND n.LastModified <= @to ";
                    queryNotesForAnsweredQuestions += " AND n.LastModified <= @to ";
                    queryForNotesAttachedToQuestionsButNoAnswers += " AND n.LastModified <= @to ";
                    parameters.Add("to", request.To ?? DateTime.Now.AddDays(2));
                }

                if (request.ObserverId.HasValue)
                {
                    queryNotesNotAttachedToQuestions += " AND obs.Id = @ObserverId ";
                    queryNotesForAnsweredQuestions += " AND obs.Id = @ObserverId ";
                    queryForNotesAttachedToQuestionsButNoAnswers += " AND obs.Id = @ObserverId ";
                    parameters.Add("ObserverId", request.ObserverId);
                }

                if (request.NgoId.HasValue)
                {
                    queryNotesNotAttachedToQuestions += " AND obs.IdNgo = @IdNgo ";
                    queryNotesForAnsweredQuestions += " AND obs.IdNgo = @IdNgo ";
                    queryForNotesAttachedToQuestionsButNoAnswers += " AND obs.IdNgo = @IdNgo ";
                    parameters.Add("IdNgo", request.NgoId);
                }

                if (!string.IsNullOrEmpty(request.County))
                {
                    queryNotesNotAttachedToQuestions += " AND obs.IdNgo = @IdNgo ";
                    queryNotesForAnsweredQuestions += " AND obs.IdNgo = @IdNgo ";
                    queryForNotesAttachedToQuestionsButNoAnswers += " AND obs.IdNgo = @IdNgo ";
                    parameters.Add("County", request.County);
                }

                if (request.PollingStationNumber.HasValue)
                {
                    // PollingStationNumber is for answers only if note does not lead to an answer do not use it
                    queryNotesNotAttachedToQuestions += " AND 1=2 ";

                    queryNotesForAnsweredQuestions += " AND a.PollingStationNumber = @PollingStationNumber ";

                    // PollingStationNumber is for answers only if note does not lead to an answer do not use it
                    queryForNotesAttachedToQuestionsButNoAnswers += " AND 1=2 ";

                    parameters.Add("PollingStationNumber", request.PollingStationNumber);
                }
            }

            var query = @$"
                {queryNotesNotAttachedToQuestions}
                UNION
                {queryNotesForAnsweredQuestions}
                UNION
                {queryForNotesAttachedToQuestionsButNoAnswers}
                ";

            IEnumerable<NotesExportModel> data = Enumerable.Empty<NotesExportModel>();
            using (var db = _context.Database.GetDbConnection())
            {
                db.Open();
                data = db.Query<NotesExportModel>(sql: query.ToString(), param: parameters, commandTimeout: 60);
            }

            return Task.FromResult(data);
        }
    }
}
