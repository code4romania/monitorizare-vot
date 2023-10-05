using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.DataExport.Models;
using VoteMonitor.Api.DataExport.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.DataExport.Handlers;

public class DataExportQueryHandler : IRequestHandler<GetDataForExport, IEnumerable<ExportModelDto>>,
    IRequestHandler<GetNotesForExport, IEnumerable<NotesExportModel>>
{
    private readonly VoteMonitorContext _context;

    public DataExportQueryHandler(VoteMonitorContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<ExportModelDto>> Handle(GetDataForExport request, CancellationToken cancellationToken)
    {
        var query = @$" SELECT
			obs.""{nameof(Observer.Phone)}"" as ObserverPhone,
			obs.""{nameof(Observer.IdNgo)}"",
			f.""{nameof(Form.Code)}"" as FormCode,
			q.""{nameof(Question.Text)}"" as QuestionText,
			o.""{nameof(Option.Text)}"" as OptionText,
			a.""{nameof(Answer.Value)}"" as AnswerFreeText,
			a.""{nameof(Answer.LastModified)}"",
			a.""{nameof(Answer.CountyCode)}"",
			a.""{nameof(Answer.PollingStationNumber)}"",
			count(n.""{nameof(Note.Text)}"") as NumberOfNotes,
			count(na.""{nameof(NotesAttachments.Id)}"") as NumberOfAttachments
		FROM 
			(public.""Answers"" a 
			INNER JOIN public.""Observers"" obs
				ON a.""{nameof(Answer.IdObserver)}"" = obs.""Id""
			INNER JOIN public.""OptionsToQuestions"" oq
				ON a.""{nameof(Answer.IdOptionToQuestion)}"" = oq.""Id""
			INNER JOIN public.""Options"" o 
				ON oq.""{nameof(OptionToQuestion.IdOption)}"" = o.""Id""
			INNER JOIN public.""Questions"" q
				ON oq.""{nameof(OptionToQuestion.IdQuestion)}"" = q.""Id""
			INNER JOIN public.""FormSections"" fs
				ON q.""{nameof(Question.IdSection)}"" = fs.""Id""
			INNER JOIN public.""Forms"" f
				ON fs.""{nameof(FormSection.IdForm)}"" = f.""Id"")
			LEFT JOIN public.""Notes"" n
				ON n.""{nameof(Note.IdQuestion)}"" = q.""Id"" AND n.""{nameof(Note.IdObserver)}"" = obs.""Id"" AND n.""{nameof(Note.IdPollingStation)}"" = a.""{nameof(Answer.IdPollingStation)}""
			LEFT JOIN public.""NotesAttachments"" na
				on na.""{nameof(NotesAttachments.NoteId)}"" = n.""Id""
		WHERE
			a.""{nameof(Answer.LastModified)}"" >= @from
                        AND obs.""{nameof(Observer.IsTestObserver)}"" = false
            ";

        var parameters = new DynamicParameters();
        parameters.Add("from", request.From ?? new DateTime(2019, 11, 08, 6, 0, 0));

        if (request.ApplyFilters)
        {
            if (request.To.HasValue)
            {
                query += @$" AND a.""{nameof(Answer.LastModified)}"" <= @to ";
                parameters.Add("to", request.To ?? DateTime.UtcNow.AddDays(2));
            }

            if (request.ObserverId.HasValue)
            {
                query += @" AND obs.""Id"" = @ObserverId ";
                parameters.Add("ObserverId", request.ObserverId);
            }

            if (request.NgoId.HasValue)
            {
                query += @$" AND obs.""{nameof(Observer.IdNgo)}"" = @IdNgo ";
                parameters.Add("IdNgo", request.NgoId);
            }

            if (!string.IsNullOrEmpty(request.County))
            {
                query += @$" AND a.""{nameof(Answer.CountyCode)}"" = @County ";
                parameters.Add("County", request.County);
            }

            if (request.PollingStationNumber.HasValue)
            {
                query += @$" AND a.""{nameof(Answer.PollingStationNumber)}"" = @PollingStationNumber ";
                parameters.Add("PollingStationNumber", request.PollingStationNumber);
            }
        }

        query = query + @$"
                group by obs.""{nameof(Observer.Phone)}"" ,
                        obs.""{nameof(Observer.IdNgo)}"",
                        f.""{nameof(Form.Code)}"",
                        q.""{nameof(Question.Text)}"",
                        o.""{nameof(Option.Text)}"",
                        a.""{nameof(Answer.Value)}"",
                        a.""{nameof(Answer.LastModified)}"",
                        a.""{nameof(Answer.CountyCode)}"",
                        a.""{nameof(Answer.PollingStationNumber)}""";

        IEnumerable<ExportModelDto> data;
        using (var db = _context.Database.GetDbConnection())
        {
            db.Open();
            data = db.Query<ExportModelDto>(sql: query, param: parameters, commandTimeout: 60);
        }

        return Task.FromResult(data);
    } 
        
    public Task<IEnumerable<NotesExportModel>> Handle(GetNotesForExport request, CancellationToken cancellationToken)
    {
        var queryNotesNotAttachedToQuestions = @$"
            SELECT obs.""{nameof(Observer.Phone)}"" AS ObserverPhone,
                obs.""{nameof(Observer.IdNgo)}"",
                '' AS FormCode,
                '' AS QuestionText,
                '' AS OptionText,
                '' AS AnswerFreeText,
                n.""{nameof(Note.Text)}"" AS NoteText,
                na.""{nameof(NotesAttachments.NoteId)}"" AS NoteAttachmentPath,
                NULL AS LastModified,
                cc.""{nameof(County.Code)}"" AS CountyCode,
                ps.""{nameof(PollingStation.Number)}"" AS PollingStationNumber
            FROM public.""Notes"" n
            INNER JOIN public.""Observers"" obs ON n.""{nameof(Note.IdObserver)}"" = obs.""Id""
            LEFT JOIN public.""NotesAttachments"" na ON na.""{nameof(NotesAttachments.NoteId)}"" = n.""Id""
            INNER JOIN public.""PollingStations"" ps ON n.""{nameof(Note.IdPollingStation)}"" = ps.""Id""
            INNER JOIN public.""Municipalities"" m ON ps.""{nameof(PollingStation.MunicipalityId)}"" = m.""Id""
            INNER JOIN public.""Counties"" cc ON m.""{nameof(Municipality.CountyId)}"" = cc.""Id""
            WHERE obs.""{nameof(Observer.IsTestObserver)}"" = false
            AND n.""{nameof(Note.IdQuestion)}"" IS NULL
            AND n.""{nameof(Note.LastModified)}"" >= @from";

        var queryNotesForAnsweredQuestions = @$"
            SELECT obs.""{nameof(Observer.Phone)}"" AS ObserverPhone,
                   obs.""{nameof(Observer.IdNgo)}"",
                   f.""{nameof(Form.Code)}"" AS FormCode,
                   q.""{nameof(Question.Text)}"" AS QuestionText,
                   o.""{nameof(Option.Text)}"" AS OptionText,
                   a.""{nameof(Answer.Value)}"" AS AnswerFreeText,
                   n.""{nameof(Note.Text)}"" AS NoteText,
                   na.""{nameof(NotesAttachments.NoteId)}"" AS NoteAttachmentPath,
                   a.""{nameof(Answer.LastModified)}"",
                   a.""{nameof(Answer.CountyCode)}"",
                   a.""{nameof(Answer.PollingStationNumber)}""
            FROM public.""Notes"" n
            INNER JOIN public.""Observers"" obs ON n.""{nameof(Note.IdObserver)}"" = obs.""Id""
            INNER JOIN public.""Questions"" q ON n.""{nameof(Note.IdQuestion)}"" = q.""Id""
            INNER JOIN public.""OptionsToQuestions"" oq ON oq.""{nameof(OptionToQuestion.IdQuestion)}"" = q.""Id""
            INNER JOIN public.""Options"" o ON oq.""{nameof(OptionToQuestion.IdOption)}"" = o.""Id""
            INNER JOIN public.""FormSections"" fs ON q.""{nameof(Question.IdSection)}"" = fs.""Id""
            INNER JOIN public.""Forms"" f ON fs.""{nameof(FormSection.IdForm)}"" = f.""Id""
            INNER JOIN public.""Answers"" a ON a.""{nameof(Answer.IdOptionToQuestion)}"" = oq.""Id""
            AND a.""{nameof(Answer.IdObserver)}"" = obs.""Id""
            AND n.""{nameof(Note.IdPollingStation)}"" = a.""{nameof(Answer.IdPollingStation)}""
            LEFT JOIN public.""NotesAttachments"" na ON na.""{nameof(NotesAttachments.NoteId)}"" = n.""Id""
            WHERE obs.""{nameof(Observer.IsTestObserver)}"" = false
            AND n.""{nameof(Note.IdQuestion)}"" IS NOT NULL
            AND n.""{nameof(Note.LastModified)}"" >= @from";

        var queryForNotesAttachedToQuestionsButNoAnswers = @$"
            SELECT obs.""{nameof(Observer.Phone)}"" AS ObserverPhone,
                   obs.""{nameof(Observer.IdNgo)}"",
                   f.""{nameof(Form.Code)}"" AS FormCode,
                   q.""{nameof(Question.Text)}"" AS QuestionText,
                   '' AS OptionText,
                   '' AS AnswerFreeText,
                   n.""{nameof(Note.Text)}"" AS NoteText,
                   na.""{nameof(NotesAttachments.NoteId)}"" AS NoteAttachmentPath,
                   NULL AS LastModified,
                   cc.""{nameof(County.Code)}"" AS CountyCode,
                   ps.""{nameof(PollingStation.Number)}"" AS PollingStationNumber
            FROM public.""Notes"" n
            INNER JOIN public.""Observers"" obs ON n.""{nameof(Note.IdObserver)}"" = obs.""Id""
            INNER JOIN public.""Questions"" q ON n.""{nameof(Note.IdQuestion)}"" = q.""Id""
            INNER JOIN public.""FormSections"" fs ON q.""{nameof(Question.IdSection)}"" = fs.""Id""
            INNER JOIN public.""Forms"" f ON fs.""{nameof(FormSection.IdForm)}"" = f.""Id""
            LEFT JOIN public.""NotesAttachments"" na ON na.""{nameof(NotesAttachments.NoteId)}"" = n.""Id""
            INNER JOIN public.""PollingStations"" ps ON n.""{nameof(Note.IdPollingStation)}"" = ps.""Id""
            INNER JOIN public.""Municipalities"" m ON ps.""{nameof(PollingStation.MunicipalityId)}"" = m.""Id""
            INNER JOIN public.""Counties"" cc ON m.""{nameof(Municipality.CountyId)}"" = cc.""Id""
            WHERE NOT EXISTS
                (SELECT *
                 FROM public.""Answers"" a
                 INNER JOIN public.""OptionsToQuestions"" oq ON oq.""Id"" = a.""{nameof(Answer.IdOptionToQuestion)}""
                 AND oq.""{nameof(OptionToQuestion.IdQuestion)}"" = q.""Id""
                 WHERE a.""{nameof(Answer.IdObserver)}"" = obs.""Id"" )
                    AND obs.""{nameof(Observer.IsTestObserver)}"" = false
                 AND n.""{nameof(Note.LastModified)}"" >= @from";

        var parameters = new DynamicParameters();
        parameters.Add("from", request.From ?? new DateTime(2019, 11, 08, 6, 0, 0));

        if (request.ApplyFilters)
        {
            if (request.To.HasValue)
            {
                queryNotesNotAttachedToQuestions += @$" AND n.""{nameof(Note.LastModified)}"" <= @to ";
                queryNotesForAnsweredQuestions += @$" AND n.""{nameof(Note.LastModified)}"" <= @to ";
                queryForNotesAttachedToQuestionsButNoAnswers += @$" AND n.""{nameof(Note.LastModified)}"" <= @to ";
                parameters.Add("to", request.To ?? DateTime.UtcNow.AddDays(2));
            }

            if (request.ObserverId.HasValue)
            {
                queryNotesNotAttachedToQuestions += @" AND obs.""Id"" = @ObserverId ";
                queryNotesForAnsweredQuestions += @" AND obs.""Id"" = @ObserverId ";
                queryForNotesAttachedToQuestionsButNoAnswers += @" AND obs.""Id"" = @ObserverId ";
                parameters.Add("ObserverId", request.ObserverId);
            }

            if (request.NgoId.HasValue)
            {
                queryNotesNotAttachedToQuestions += @$" AND obs.""{nameof(Observer.IdNgo)}"" = @IdNgo ";
                queryNotesForAnsweredQuestions += @$" AND obs.""{nameof(Observer.IdNgo)}"" = @IdNgo ";
                queryForNotesAttachedToQuestionsButNoAnswers += @$" AND obs.""{nameof(Observer.IdNgo)}"" = @IdNgo ";
                parameters.Add("IdNgo", request.NgoId);
            }

            if (!string.IsNullOrEmpty(request.County))
            {
                queryNotesNotAttachedToQuestions += @$" AND obs.""{nameof(Observer.IdNgo)}"" = @IdNgo ";
                queryNotesForAnsweredQuestions += @$" AND obs.""{nameof(Observer.IdNgo)}"" = @IdNgo ";
                queryForNotesAttachedToQuestionsButNoAnswers += @$" AND obs.""{nameof(Observer.IdNgo)}"" = @IdNgo ";
                parameters.Add("County", request.County);
            }

            if (request.PollingStationNumber.HasValue)
            {
                // PollingStationNumber is for answers only if note does not lead to an answer do not use it
                queryNotesNotAttachedToQuestions += " AND 1=2 ";

                queryNotesForAnsweredQuestions += @$" AND a.""{nameof(PollingStation.Number)}"" = @PollingStationNumber ";

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

        IEnumerable<NotesExportModel> data;
        using (var db = _context.Database.GetDbConnection())
        {
            db.Open();
            data = db.Query<NotesExportModel>(sql: query, param: parameters, commandTimeout: 60);
        }

        return Task.FromResult(data);
    }
}
