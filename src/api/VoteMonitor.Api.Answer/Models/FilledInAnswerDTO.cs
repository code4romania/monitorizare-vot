namespace VoteMonitor.Api.Answer.Models
{
    public class FilledInAnswerDto
    {
        public int OptionId { get; set; }
        public string Text { get; set; }
        public bool IsFreeText { get; set; }
        public string Value { get; set; }
        public bool IsFlagged { get; set; }
    }
}
