using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using VoteMonitor.Api.Core.Attributes;

namespace VoteMonitor.Api.County.Models
{
    public class CountiesUploadRequest
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".csv" })]
        public IFormFile CsvFile { get; set; }
    }
}