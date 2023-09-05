using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using VoteMonitor.Api.Core.Attributes;

namespace VoteMonitor.Api.Location.Models;

public class PollingStationsUploadRequest
{
    [Required(ErrorMessage = "Please select a file.")]
    [DataType(DataType.Upload)]
    [AllowedExtensions(new string[] { ".csv" })]
    public IFormFile CsvFile { get; set; }
}