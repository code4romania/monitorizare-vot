using System.Collections.Generic;

namespace VoteMonitor.Api.Answer.Models
{
    public class QuestionDTO<T>
        where T : class
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int IdQuestionType { get; set; }
        public string IdQuestion { get; set; }
        public string FormCode { get; set; }

        public IList<T> Answers { get; set; }
    }
}