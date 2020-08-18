using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using VoteMonitor.Api.Core.Attributes;

namespace VoteMonitor.Api.Note.Models
{
    public class UploadNoteModel
    {
        [Required(AllowEmptyStrings = false)] 
        public string CountyCode { get; set; }

        [Required] 
        public int PollingStationNumber { get; set; }

        public int? QuestionId { get; set; }

        public string Text { get; set; }

        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        public IFormFile File { get; set; }
    }
}