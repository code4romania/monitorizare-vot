using MediatR;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace VoteMonitor.Api.Core.Commands
{
    public class UploadFileCommandV2 : IRequest<string[]>
    {
        public List<IFormFile> Files { get; set; }
        public UploadType UploadType { get; set; }
    }
}