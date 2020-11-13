using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Models
{
    public class OptionToQuestionDTO : IIdentifiableEntity
    {
        public int Id { get; set; }
        public int IdOption { get; set; }
        public string Text { get; set; }
        public bool IsFreeText { get; set; }
        public bool Flagged { get; set; }
        public int OrderNumber { get; set; }
    }
}