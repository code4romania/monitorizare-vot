namespace VotingIrregularities.Api.Models.Forms {
    public class OptionToQuestionDTO {
        public int IdOption { get; set; }
        public string Text { get; set; }
        public bool IsFreeText { get; set; }
    }
}