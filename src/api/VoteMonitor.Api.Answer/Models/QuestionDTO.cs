using System.Collections.Generic;

namespace VoteMonitor.Api.Answer.Models
{
    public class QuestionDto<T>
        where T : class
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionTypeId { get; set; }
        public string QuestionId { get; set; }
        public string FormCode { get; set; }

        public IList<T> Answers { get; set; }
    }
}
