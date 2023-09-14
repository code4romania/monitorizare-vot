using MediatR;
using Microsoft.AspNetCore.Http;

namespace VoteMonitor.Api.Core.Commands;

public class UploadFileCommandV2 : IRequest<string[]>
{
    public UploadFileCommandV2(List<IFormFile> files, UploadType uploadType)
    {
        Files = files.ToList().AsReadOnly();
        UploadType = uploadType;
    }

    public IReadOnlyCollection<IFormFile> Files { get; }
    public UploadType UploadType { get; }
}
