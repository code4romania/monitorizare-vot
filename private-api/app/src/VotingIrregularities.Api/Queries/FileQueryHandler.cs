using System.IO;
using MediatR;
using VotingIrregularities.Api.Services;
using VotingIrregularities.Api.Models;
using System.Threading.Tasks;

namespace VotingIrregularities.Api.Queries
{
    public class FileQueryHandler : IAsyncRequestHandler<ModelFile, string>
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
        public async Task<string> Handle(ModelFile message)
        {
            if(message.File != null)
                return await _fileService.UploadFromStreamAsync(message.File.OpenReadStream(), message.File.ContentType,Path.GetExtension(message.File.FileName));

            return string.Empty;
        }
    }
}
