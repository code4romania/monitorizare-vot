using System.Collections.Generic;

namespace VotingIrregularities.Api.Models
{
    public class QuestionModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int QuestionTypeId { get; set; }
        public string QuestionCode { get; set; }

        public List<AvailableAnswersModel> AvailableAnswers { get; set; }
    }
}
