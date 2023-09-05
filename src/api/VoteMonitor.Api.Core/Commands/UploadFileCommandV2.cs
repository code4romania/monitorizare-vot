using MediatR;
using Microsoft.AspNetCore.Http;

namespace VoteMonitor.Api.Core.Commands;

public class UploadFileCommandV2 : IRequest<string[]>
{
    public List<IFormFile> Files { get; set; }
    public UploadType UploadType { get; set; }
}