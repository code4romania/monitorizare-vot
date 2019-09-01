using MediatR;
using Microsoft.AspNetCore.Http;

namespace VoteMonitor.Api.Note.Commands
{
    public class UploadFileCommand : IRequest<string>
    {
        public IFormFile File { get; set; }
    }
}