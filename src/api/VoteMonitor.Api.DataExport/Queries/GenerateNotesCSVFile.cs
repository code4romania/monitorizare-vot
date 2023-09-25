using MediatR;
using VoteMonitor.Api.DataExport.Models;

namespace VoteMonitor.Api.DataExport.Queries;

public class GenerateNotesCSVFile : IRequest<byte[]>
{
    public IReadOnlyCollection<NotesExportModel> Data { get; }

    public GenerateNotesCSVFile(IEnumerable<NotesExportModel> data)
    {
        Data = data.ToList().AsReadOnly();
    }
}
