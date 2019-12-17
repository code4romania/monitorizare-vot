using MediatR;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Core.Services;

namespace VoteMonitor.Api.Core.Handlers
{
    public class UploadFileHandler : IRequestHandler<UploadFileCommand, string>
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
        public async Task<string> Handle(UploadFileCommand message, CancellationToken cancellationToken)
        {
            if (message.File != null)
            {
                return await _fileService.UploadFromStreamAsync(message.File.OpenReadStream(),
                    message.File.ContentType,
                    Path.GetExtension(message.File.FileName),
                    message.UploadType);
            }

            return string.Empty;
        }
    }
}
