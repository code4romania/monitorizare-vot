using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Answer.Models
{
    public class BulkAnswerDto
    {
        [Required]
        public int QuestionId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CountyCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int PollingStationNumber { get; set; }

        public List<SelectedOptionDto> Options { get; set; }
    }
}
