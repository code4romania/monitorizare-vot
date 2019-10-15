using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MonitorizareVot.Api.Location.Models;

namespace MonitorizareVot.Api.Location.Services
{
    public interface IFileLoader
    {
        Task<List<PollingStationDTO>> ImportFileAsync(IFormFile file);

        bool ValidateFile(IFormFile file);
    }
} 