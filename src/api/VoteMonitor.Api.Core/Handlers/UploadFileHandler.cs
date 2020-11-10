using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Core.Services;

namespace VoteMonitor.Api.Core.Handlers
{
    public class UploadFileHandler : IRequestHandler<UploadFileCommandV2, string[]>,
        IRequestHandler<UploadFileCommand, string>
    {
        private readonly IFileService _fileService;

        public UploadFileHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        ///  Uploads a list of files in blob storage
        /// </summary>
        /// <returns>The url of the blob</returns>
        public async Task<string[]> Handle(UploadFileCommandV2 message, CancellationToken cancellationToken)
        {
            if (message.Files != null && message.Files.Any())
            {
                List<Task<string>> uploadTasks = new List<Task<string>>();

                foreach (var file in message.Files)
                {
                    var uploadTask =  _fileService.UploadFromStreamAsync(file.OpenReadStream(),
                    file.ContentType,
                    Path.GetExtension(file.FileName),
                    message.UploadType);

                    uploadTasks.Add(uploadTask);
                }

                string[] files = await Task.WhenAll(uploadTasks);

                return files;
            }

            return new string[0];
        }

        [Obsolete("Will be removed when ui will use multiple files upload")]
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