using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.DataExport.Models;

namespace VoteMonitor.Api.DataExport.Queries
{
    public class GenerateNotesCSVFile : IRequest<byte[]>
    {
        public IEnumerable<NotesExportModel> Data { get; }

        public GenerateNotesCSVFile(IEnumerable<NotesExportModel> data)
        {
            Data = data;
        }
    }

}