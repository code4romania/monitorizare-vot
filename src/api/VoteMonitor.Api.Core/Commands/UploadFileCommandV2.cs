using MediatR;
using Microsoft.AspNetCore.Http;
using VoteMonitor.Api.Core.Models;

namespace VoteMonitor.Api.Core.Commands;

public class UploadFileCommandV2 : IRequest<UploadedFileModel[]>
{
    public UploadFileCommandV2(List<IFormFile> files, UploadType uploadType)
    {
        Files = files.ToList().AsReadOnly();
        UploadType = uploadType;
    }

    public IReadOnlyCollection<IFormFile> Files { get; }
    public UploadType UploadType { get; }
}
