using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoteMonitor.Api.Location.Models;

namespace VoteMonitor.Api.Location.Services
{
    public interface IFileLoader
    {
        Task<List<PollingStationDTO>> ImportFileAsync(IFormFile file);

        bool ValidateFile(IFormFile file);

        byte[] ExportHeaderInformation();
    }
}