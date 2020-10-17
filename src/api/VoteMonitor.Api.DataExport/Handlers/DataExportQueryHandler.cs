﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.DataExport.Queries;
using VoteMonitor.Entities;
using Dapper;
using System.Linq;

namespace VoteMonitor.Api.DataExport.Handlers
{
    public class DataExportQueryHandler : IRequestHandler<GetDataForExport, IEnumerable<ExportModel>>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;

        public DataExportQueryHandler(VoteMonitorContext context, ILogger<DataExportQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<IEnumerable<ExportModel>> Handle(GetDataForExport request, CancellationToken cancellationToken)
        {
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
            AND obs.IsTestObserver = 0
            ";

            DynamicParameters parameters = new DynamicParameters();
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

            IEnumerable<ExportModel> data = Enumerable.Empty<ExportModel>();
            using (var db = _context.Database.GetDbConnection())
            {
                try
                {
                    db.Open();
                    data = db.Query<ExportModel>(sql: query.ToString(), param: parameters, commandTimeout: 60);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, nameof(Handle));
                }
                finally
                {
                    db.Close();
                }
            }
            return Task.FromResult(data);
        }
    }
}