using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.DataExport.Queries;
using VoteMonitor.Entities;
using System.Text;
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
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("From", request.From ?? new DateTime(2019, 11, 08, 6, 0, 0));

            // buld query
            var query = new StringBuilder()
                .AppendLine(";WITH FilteredAnswer AS (")
                .AppendLine("	SELECT")
                .AppendLine("		[Value] AS AnswerFreeText")
                .AppendLine("		, LastModified")
                .AppendLine("		, CountyCode")
                .AppendLine("		, PollingStationNumber")
                .AppendLine("		, IdObserver")
                .AppendLine("		, IdOptionToQuestion")
                .AppendLine("		, IdPollingStation")
                .AppendLine("	FROM Answers")
                .AppendLine("	WHERE ")
                .AppendLine("		LastModified >= @From");

            // request.To
            if (request.ApplyFilters && request.To.HasValue)
            {
                query.AppendLine("		AND LastModified <= @To");
                parameter.Add("To", request.To ?? DateTime.Now.AddDays(2));
            }
            // request.County
            if (request.ApplyFilters && !string.IsNullOrEmpty(request.County))
            {
                query.AppendLine("		AND CountyCode = @CountyCode");
                parameter.Add("CountyCode", request.County);
            }
            // request.PollingStationNumber
            if (request.ApplyFilters && request.PollingStationNumber.HasValue)
            {
                query.AppendLine("		AND PollingStationNumber = @PollingStationNumber");
                parameter.Add("PollingStationNumber", request.PollingStationNumber);
            }

            query
                .AppendLine(")") // ends FilteredAnswer
                .AppendLine(", FilteredObservers AS (")
                .AppendLine("	SELECT")
                .AppendLine("		Id")
                .AppendLine("		, Phone as ObserverPhone")
                .AppendLine("		, IdNgo")
                .AppendLine("	FROM Observers")
                .AppendLine("	WHERE ")
                .AppendLine("		IsTestObserver = 0");

            // request.ObserverId
            if (request.ApplyFilters && request.ObserverId.HasValue)
            {
                query.AppendLine("		AND Id = @ObserverId");
                parameter.Add("ObserverId", request.ObserverId);
            }
            // request.NgoId
            if (request.ApplyFilters && request.NgoId.HasValue)
            {
                query.AppendLine("		AND IdNgo = @IdNgo");
                parameter.Add("IdNgo", request.NgoId);
            }

            query
                .AppendLine(")") // ends FilteredObservers
                .AppendLine(", PreFiltered AS (")
                .AppendLine(" SELECT  ")
                .AppendLine("	obs.ObserverPhone")
                .AppendLine("	, obs.IdNgo")
                .AppendLine("	, f.Code as FormCode")
                .AppendLine("	, q.Text as QuestionText")
                .AppendLine("	, o.Text as OptionText")
                .AppendLine("	, a.AnswerFreeText")
                .AppendLine("	, a.CountyCode")
                .AppendLine("	, a.PollingStationNumber")
                .AppendLine("	, q.Id AS QuestionId")
                .AppendLine("	, obs.Id AS ObserverId")
                .AppendLine("	, a.IdPollingStation")
                .AppendLine("	FROM ")
                .AppendLine("		FilteredAnswer AS a")
                .AppendLine("		INNER JOIN FilteredObservers AS obs ON a.IdObserver = obs.Id")
                .AppendLine("		INNER JOIN OptionsToQuestions AS oq ON a.IdOptionToQuestion = oq.Id")
                .AppendLine("		INNER JOIN Options AS o ON oq.IdOption = o.Id")
                .AppendLine("		INNER JOIN Questions AS q ON oq.IdQuestion = q.Id")
                .AppendLine("		INNER JOIN FormSections AS fs ON q.IdSection = fs.Id")
                .AppendLine("		INNER JOIN Forms f ON fs.IdForm = f.Id")
                .AppendLine(")")
                .AppendLine("SELECT ")
                .AppendLine("    NEWID() as Id ")
                .AppendLine("	, ObserverPhone")
                .AppendLine("	, IdNgo")
                .AppendLine("	, FormCode")
                .AppendLine("	, QuestionText")
                .AppendLine("	, OptionText")
                .AppendLine("	, AnswerFreeText")
                .AppendLine("	, n.Text as NoteText")
                .AppendLine("	, n.AttachementPath as NoteAttachmentPath")
                .AppendLine("	, LastModified")
                .AppendLine("	, CountyCode")
                .AppendLine("	, PollingStationNumber")
                .AppendLine("FROM PreFiltered")
                .AppendLine("LEFT JOIN Notes AS n ON n.IdQuestion = QuestionId AND n.IdObserver = ObserverId AND n.IdPollingStation = PreFiltered.IdPollingStation");

            IEnumerable<ExportModel> data = Enumerable.Empty<ExportModel>();
            using (var db = _context.Database.GetDbConnection())
            {
                try
                {
                    db.Open();
                    data = db.Query<ExportModel>(sql: query.ToString(), param: parameter, commandTimeout: 60);
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