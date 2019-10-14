using System.Collections.Generic;

namespace VoteMonitor.Api.Answer.Models {
    public class AnswerDTO {
        public int QuestionId { get; set; }
        public int PollingSectionId { get; set; }
        public int FormId { get; set; }
        public string CountyCode { get; set; }
        public int PollingStationNumber { get; set; }
        public List<SelectedOptionModel> Options { get; set; }
    }
}
