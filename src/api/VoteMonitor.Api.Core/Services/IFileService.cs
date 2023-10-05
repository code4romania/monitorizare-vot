using VoteMonitor.Api.Core.Models;

namespace VoteMonitor.Api.Core.Services;

/// <summary>
/// Interface for the file service to be used
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Upload stream into file storage
    /// </summary>
    /// <param name="sourceStream"></param>
    /// <param name="contentType"></param>
    /// <param name="extension"></param>
    /// <returns>the reference to the resource just uploaded</returns>
    Task<UploadedFileModel> UploadFromStreamAsync(Stream sourceStream, string contentType, string extension, UploadType uploadType);

    /// <summary>
    /// Gets pre-signed url for a given file
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    Task<string> GetPreSignedUrl(string filename);
}
