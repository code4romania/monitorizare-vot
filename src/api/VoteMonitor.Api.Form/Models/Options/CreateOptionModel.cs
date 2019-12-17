using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Form.Models.Options
{
    public class CreateOptionModel
    {
        [Required]
        public bool IsFreeText { get; set; }

        [Required(AllowEmptyStrings = false), MaxLength(1000)]
        public string Text { get; set; }

        public string Hint { get; set; }
    }
}
