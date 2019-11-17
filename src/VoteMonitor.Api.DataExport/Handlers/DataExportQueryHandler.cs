using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.DataExport.Model;
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
            var exportData = await _context.Answers
                  .Where(a => a.IdObserver > 10)
                  .Where(a => a.LastModified >= new DateTime(2019, 11, 08, 6, 0, 0))
                  .Where(a => a.Observer.IdNgo != 1)
                  .Where(a => a.OptionAnswered != null && a.OptionAnswered.Question != null)
                  .SelectMany(a => a.OptionAnswered.Question.Notes.DefaultIfEmpty(), (a, note) => new ExportModel
                  {
                      ObserverPhone = a.Observer.Phone,
                      IdNgo = a.Observer.IdNgo,
                      FormCode = a.OptionAnswered.Question.FormSection.Form.Code,
                      QuestionText = a.OptionAnswered.Question.Text,
                      OptionText = a.OptionAnswered.Option.Text,
                      AnswerFreeText = a.Value,
                      NoteText = note.Text,
                      NoteAttachmentPath = note.AttachementPath,
                      LastModified = a.LastModified,
                      CountyCode = a.CountyCode,
                      PollingStationNumber = a.PollingStationNumber
                  })
                  .ToListAsync(cancellationToken);

            return exportData;
        }
    }
}