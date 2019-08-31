using MediatR;
using Microsoft.AspNetCore.Http;

namespace VoteMonitor.Api.Note.Models
{
    public class FileModel : IRequest<string>
    {
        public IFormFile File { get; set; }
    }
}