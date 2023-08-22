using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Options;

namespace VoteMonitor.Api.Core.Services
{
    /// <summary>
    /// This will be used just for development purposes
    /// </summary>
    public class LocalFileService : IFileService
    {
        private readonly LocalFileStorageOptions _localFileOptions;

        /// <summary>
        /// Constructor for dependency injection
        /// </summary>
        /// <param name="options"></param>
        public LocalFileService(IOptions<LocalFileStorageOptions> options)
        {
            _localFileOptions = options.Value;
        }
        public Task<string> UploadFromStreamAsync(Stream sourceStream, string mimeType, string extension, UploadType uploadType)
        {
            var uploadDirectory = _localFileOptions.StoragePaths[uploadType.ToString()];

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            // set name
            var localFile = Path.Combine(uploadDirectory, Guid.NewGuid().ToString("N") + extension);

            // save to local path
            using (var fileStream = File.Create(localFile))
            {
                sourceStream.Seek(0, SeekOrigin.Begin);
                sourceStream.CopyTo(fileStream);
            }

            // return relative path
            return Task.FromResult(localFile);
        }
    }
}
