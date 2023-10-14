using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using VoteMonitor.Api.Core.Models;
using VoteMonitor.Api.Core.Options;

namespace VoteMonitor.Api.Core.Services;

/// <summary>
/// This will be used just for development purposes
/// </summary>
public class LocalFileService : IFileService
{
    private readonly LocalFileStorageOptions _localFileOptions;

    private readonly ConcurrentDictionary<string, string> _uploadedFiles = new();

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="options"></param>
    public LocalFileService(IOptions<LocalFileStorageOptions> options)
    {
        _localFileOptions = options.Value;
    }
    public Task<UploadedFileModel> UploadFromStreamAsync(Stream sourceStream, string contentType, string extension, UploadType uploadType)
    {
        var uploadDirectory = _localFileOptions.StoragePaths[uploadType.ToString()];

        if (!Directory.Exists(uploadDirectory))
        {
            Directory.CreateDirectory(uploadDirectory);
        }

        // set name
        var fileName = Guid.NewGuid().ToString("N") + extension;
        var localFilePath = Path.Combine(uploadDirectory, fileName);

        // save to local path
        using (var fileStream = File.Create(localFilePath))
        {
            sourceStream.Seek(0, SeekOrigin.Begin);
            sourceStream.CopyTo(fileStream);
        }

        _uploadedFiles.AddOrUpdate(fileName, localFilePath, (_, _) => localFilePath);

        // return relative path
        return Task.FromResult(new UploadedFileModel()
        {
            FileName = fileName,
            Path = localFilePath
        });
    }

    public string GetPreSignedUrl(string filename)
    {
        if (_uploadedFiles.TryGetValue(filename, out var path))
        {
            return path;
        }

        return string.Empty;
    }
}
