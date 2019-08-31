using System.IO;
using MediatR;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Note.Models;

namespace VotingIrregularities.Api.Queries
{
    public class FileQueryHandler : AsyncRequestHandler<FileModel, string>
    {
        IFileService _fileService;

        public FileQueryHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        ///  Uploads a file in azure blob storage
        /// </summary>
        /// <returns>The url of the blob</returns>
        protected override async Task<string> HandleCore(FileModel message)
        {
            if(message.File != null)
                return await _fileService.UploadFromStreamAsync(message.File.OpenReadStream(), message.File.ContentType,Path.GetExtension(message.File.FileName));

            return string.Empty;
        }
    }
}
