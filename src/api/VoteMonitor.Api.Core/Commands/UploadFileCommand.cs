using MediatR;
using Microsoft.AspNetCore.Http;

namespace VoteMonitor.Api.Core.Commands;

[Obsolete("Will be removed when ui will use multiple files upload")]
public class UploadFileCommand : IRequest<string>
{  
    public IFormFile File { get; set; }
    public UploadType UploadType { get; set; }
}