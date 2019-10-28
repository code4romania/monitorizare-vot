using MediatR;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace VoteMonitor.Api.Core.Commands
{
    public class UploadFileCommand : IRequest<List<string>>
    {
        public List<IFormFile> Files { get; set; }
        public UploadType UploadType { get; set; }
    }
}