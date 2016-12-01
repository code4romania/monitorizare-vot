using MediatR;
using VotingIrregularities.Api.Services;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Domain.FileAggregate;
using System.Threading.Tasks;

namespace VotingIrregularities.Api.Queries
{
    public class FileQueryHandler : IAsyncRequestHandler<ModelFile, AdaugaFileCommand>
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
        public async Task<AdaugaFileCommand> Handle(ModelFile message)
        {
            string url = await _fileService.UploadFromStreamAsync(message.File.OpenReadStream(), message.File.ContentType);

            return new AdaugaFileCommand { Url = url };
        }
    }
}
