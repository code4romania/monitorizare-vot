using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VoteMonitor.Api.Note.Models;

[Obsolete("Will be removed when ui will use multiple files upload")]
public class UploadNoteModel
{
    [Required(AllowEmptyStrings = false)]
    public string CountyCode { get; set; }

    [Required]
    public int PollingStationNumber { get; set; }

    public int? QuestionId { get; set; }

    public string Text { get; set; }

    [DataType(DataType.Upload)]
    public IFormFile File { get; set; }
}