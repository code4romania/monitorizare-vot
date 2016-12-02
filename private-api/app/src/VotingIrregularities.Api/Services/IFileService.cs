using System.IO;
using System.Threading.Tasks;

namespace VotingIrregularities.Api.Services
{
    public interface IFileService
    {
        Task<string> UploadFromStreamAsync(Stream sourceStream, string mimeType, string extension);
    }
}
