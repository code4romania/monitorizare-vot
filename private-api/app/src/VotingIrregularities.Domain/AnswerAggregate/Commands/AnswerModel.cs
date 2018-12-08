using System.Collections.Generic;

namespace VotingIrregularities.Domain.AnswerAggregate.Commands
{
    public class AnswerModel
    {
        public int QuestionId { get; set; }
        public int SectionId { get; set; }
        public string FormCode { get; set; }
        public string CountyCode { get; set; }
        public int SectionNumber { get; set; }

        public List<SelectedOptionsModel> Options { get; set; }
    }
}
