using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Form.Models
{
    public class OptionDTO
    {
        public int Id { get; set; }
        public bool IsFreeText { get; set; }
        [Required, MaxLength(1000)]
        public string Text { get; set; }
        public string Hint { get; set; }
    }
}