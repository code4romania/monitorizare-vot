using System.IO;
using System.Threading.Tasks;

namespace VotingIrregularities.Api.Services
{
    /// <summary>
    /// Interface for the file service to be used
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Upload stream into file storage
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="mimeType"></param>
        /// <param name="extension"></param>
        /// <returns>the reference to the resource just uploaded</returns>
        Task<string> UploadFromStreamAsync(Stream sourceStream, string mimeType, string extension);

        /// <summary>
        /// Initialize the file system before first usage
        /// </summary>
        /// <returns></returns>
        Task Initialize();
    }
}
