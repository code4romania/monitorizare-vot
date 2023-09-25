using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VoteMonitor.Api.Note.Models;

public class UploadNoteModelV2
{
    [Required(AllowEmptyStrings = false)] 
    public string CountyCode { get; set; }

    [Required(AllowEmptyStrings = false)] 
    public string MunicipalityCode { get; set; }

    [Required] 
    public int PollingStationNumber { get; set; }

    public int? QuestionId { get; set; }

    public string Text { get; set; }

    [DataType(DataType.Upload)]
    public List<IFormFile> Files { get; set; }
}
