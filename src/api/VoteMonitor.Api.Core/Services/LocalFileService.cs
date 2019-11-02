using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VotingIrregularities.Api.Options;

namespace VoteMonitor.Api.Core.Services
{
    /// <summary>
    /// This will be used just for development purposes
    /// </summary>
    public class LocalFileService : IFileService
    {
        private readonly FileServiceOptions _localFileOptions;

        /// <summary>
        /// Constructor for dependency injection
        /// </summary>
        /// <param name="options"></param>
        public LocalFileService(IOptions<FileServiceOptions> options)
        {
            _localFileOptions = options.Value;
        }
        public Task<string> UploadFromStreamAsync(Stream sourceStream, string mimeType, string extension, UploadType uploadType)
        {
            // set name
            var localFile = _localFileOptions.StoragePaths[uploadType.ToString()] + "\\" + Guid.NewGuid().ToString("N") + extension;

            // save to local path
            using (var fileStream = File.Create(localFile))
            {
                sourceStream.Seek(0, SeekOrigin.Begin);
                sourceStream.CopyTo(fileStream);
            }

            // return relative path
            return Task.FromResult(localFile);
        }

        public Task Initialize()
        {
            return null;
        }
    }
}
