using MediatR;
using Microsoft.AspNetCore.Http;
using VoteMonitor.Api.Location.Models.ResultValues;

namespace VoteMonitor.Api.Location.Commands;

public class ImportPollingStationsCommand : IRequest<PollingStationImportResultValue>
{
    public IFormFile File { get; }

    public ImportPollingStationsCommand(IFormFile file)
    {
        File = file;
    }
}