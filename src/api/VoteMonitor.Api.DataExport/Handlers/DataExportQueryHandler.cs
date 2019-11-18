using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.DataExport.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.DataExport.Handlers
{
    public class DataExportQueryHandler : IRequestHandler<GetDataForExport, List<ExportModel>>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;

        public DataExportQueryHandler(VoteMonitorContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ExportModel>> Handle(GetDataForExport request, CancellationToken cancellationToken)
        {
            //var exportData = await _context.Answers
            //      .Where(a => a.IdObserver > 10)
            //      .Where(a => a.LastModified >= new DateTime(2019, 11, 08, 6, 0, 0))
            //      .Where(a => a.Observer.IdNgo != 1)
            //      .Where(a => a.OptionAnswered != null && a.OptionAnswered.Question != null)
            //      .SelectMany(a => a.OptionAnswered.Question.Notes.DefaultIfEmpty(), (a, note) => new ExportModel
            //      {
            //          ObserverPhone = a.Observer.Phone,
            //          IdNgo = a.Observer.IdNgo,
            //          FormCode = a.OptionAnswered.Question.FormSection.Form.Code,
            //          QuestionText = a.OptionAnswered.Question.Text,
            //          OptionText = a.OptionAnswered.Option.Text,
            //          AnswerFreeText = a.Value,
            //          NoteText = note.Text,
            //          NoteAttachmentPath = note.AttachementPath,
            //          LastModified = a.LastModified,
            //          CountyCode = a.CountyCode,
            //          PollingStationNumber = a.PollingStationNumber
            //      })
            //      .ToListAsync(cancellationToken);

            var query = @" SELECT
            NEWID() as Id,   
			obs.Phone as [ObserverPhone],
			obs.IdNgo,
			f.Code as FormCode,
			q.Text as QuestionText,
			o.Text as [OptionText],
			a.[Value] as [AnswerFreeText],
			n.Text as NoteText,
			n.AttachementPath as [NoteAttachmentPath],
			a.LastModified,
			a.CountyCode,
			a.PollingStationNumber
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
		WHERE
			a.LastModified >= @from
			AND a.IdObserver > @IdObserverLimit
            
            ";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@from", request.From ?? new DateTime(2019, 11, 08, 6, 0, 0)),
                //TODO remove limit for observer Id
                new SqlParameter("@IdObserverLimit", 10),

            };

            if (request.ApplyFilters)
            {
                if (request.To.HasValue)
                {
                    query += " AND a.LastModified <= @to ";
                    parameters.Add(new SqlParameter("@to", request.To ?? DateTime.Now.AddDays(2)));
                }

                if (request.ObserverId.HasValue)
                {
                    query += " AND obs.Id = @ObserverId ";
                    parameters.Add(new SqlParameter("@ObserverId", request.ObserverId));
                }

                if (request.NgoId.HasValue)
                {
                    query += " AND obs.IdNgo = @IdNgo ";
                    parameters.Add(new SqlParameter("@IdNgo", request.NgoId));
                }

                if (!string.IsNullOrEmpty(request.County))
                {
                    query += " AND a.CountyCode = @County ";
                    parameters.Add(new SqlParameter("@County", request.County));
                }

                if (request.PollingStationNumber.HasValue)
                {
                    query += " AND a.PollingStationNumber = @PollingStationNumber ";
                    parameters.Add(new SqlParameter("@PollingStationNumber", request.PollingStationNumber));
                }
            }

            var exportData = _context.ExportModels.FromSql(query, parameters.ToArray());

            return await exportData.ToListAsync(cancellationToken);
        }
    }
}