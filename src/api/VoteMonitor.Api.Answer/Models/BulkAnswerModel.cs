using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Answer.Models {
    public class AnswerModelWrapper {
        public BulkAnswerModel[] Answers { get; set; }
    }
    public class SelectedOptionModel {
        public int OptionId { get; set; }
        public string Value { get; set; }
    }
    public class BulkAnswerModel {
        [Required]
        public int QuestionId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CountyCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int PollingStationNumber { get; set; }

        //[Required(AllowEmptyStrings = false)]
        public int FormId { get; set; }
        public List<SelectedOptionModel> Options { get; set; }
    }
}
