using MediatR;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace VoteMonitor.Api.Note.Commands
{
    public class UploadFileCommand : IRequest<List<string>>
    {
        public List<IFormFile> Files { get; set; }
    }
}