using MediatR;
using VoteMonitor.Api.DataExport.Models;

namespace VoteMonitor.Api.DataExport.Queries;

public class GenerateCSVFile : IRequest<byte[]>
{
    public IEnumerable<ExportModelDto> Data { get; }

    public GenerateCSVFile(IEnumerable<ExportModelDto> data)
    {
        Data = data;
    }
}