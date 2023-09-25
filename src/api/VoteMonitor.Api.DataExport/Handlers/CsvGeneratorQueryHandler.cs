using MediatR;
using VoteMonitor.Api.DataExport.FileGenerator;
using VoteMonitor.Api.DataExport.Queries;

namespace VoteMonitor.Api.DataExport.Handlers;

public class CsvGeneratorQueryHandler : IRequestHandler<GenerateCSVFile, byte[]>,
    IRequestHandler<GenerateNotesCSVFile, byte[]>
{
    private readonly ICsvGenerator _csvGenerator;

    public CsvGeneratorQueryHandler()
    {
        _csvGenerator = new CsvGenerator();
    }

    public Task<byte[]> Handle(GenerateCSVFile request, CancellationToken cancellationToken)
    {
        var fileContents = _csvGenerator.Export(request.Data, "myData");

        return Task.FromResult(fileContents);
    }

    public Task<byte[]> Handle(GenerateNotesCSVFile request, CancellationToken cancellationToken)
    {
        var fileContents = _csvGenerator.Export(request.Data, "notes");

        return Task.FromResult(fileContents);
    }
}
