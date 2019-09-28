using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Note.Commands;

namespace VoteMonitor.Api.Note.Handlers
{
    public class UploadFileHandler : AsyncRequestHandler<UploadFileCommand, List<string>>
    {
        private readonly IFileService _fileService;

        public UploadFileHandler(IFileService fileService)
            => _fileService = fileService;

        /// <summary>
        ///  Uploads a file in azure blob storage
        /// </summary>
        /// <returns>The url of the blob</returns>
        protected override async Task<List<string>> HandleCore(UploadFileCommand message)
        {
            var blobUrisTasks = new List<Task<string>>();
            if (message.Files?.Any() == true)
            {
                message.Files.ForEach(file => blobUrisTasks.Add(UploadFileToAzureBlob(file)));
                return (await Task.WhenAll(blobUrisTasks)).ToList();
            }

            return null;
        }

        private async Task<string> UploadFileToAzureBlob(IFormFile file)
           => await _fileService.UploadFromStreamAsync(file.OpenReadStream(), file.ContentType, Path.GetExtension(file.FileName));
    }
}