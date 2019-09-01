using System.IO;
using System.Threading.Tasks;
using MediatR;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Note.Commands;

namespace VoteMonitor.Api.Note.Handlers
{
    public class UploadFileHandler : AsyncRequestHandler<UploadFileCommand, string>
    {
        private readonly IFileService _fileService;

        public UploadFileHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        ///  Uploads a file in azure blob storage
        /// </summary>
        /// <returns>The url of the blob</returns>
        protected override async Task<string> HandleCore(UploadFileCommand message)
        {
            if (message.File != null)
                return await _fileService.UploadFromStreamAsync(message.File.OpenReadStream(),
                    message.File.ContentType,
                    Path.GetExtension(message.File.FileName));

            return string.Empty;
        }
    }
}