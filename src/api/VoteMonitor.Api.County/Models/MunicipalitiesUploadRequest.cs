using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using VoteMonitor.Api.Core.Attributes;

namespace VoteMonitor.Api.County.Models;

public class MunicipalitiesUploadRequest
{
    [Required(ErrorMessage = "Please select a file.")]
    [DataType(DataType.Upload)]
    [AllowedExtensions(new[] { ".csv" })]
    public IFormFile CsvFile { get; set; }
}
