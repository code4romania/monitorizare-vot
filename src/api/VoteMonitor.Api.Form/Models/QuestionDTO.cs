using System.Collections.Generic;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Models
{
    public class QuestionDTO
    {
        public QuestionDTO()
        {
            OptionsToQuestions = new List<OptionToQuestionDTO>();
        }

        public int Id { get; set; }
        public string FormCode { get; set; }
        public string Code { get; set; }
        public int IdSection { get; set; }
        public QuestionType QuestionType { get; set; }
        public string Text { get; set; }
        public string Hint { get; set; }
        public int OrderNumber { get; set; }

        public IEnumerable<OptionToQuestionDTO> OptionsToQuestions { get; set; }
    }
}