using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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

    public class BulkAnswers : IRequest<CompleteazaRaspunsCommand> {
        public BulkAnswers(IEnumerable<BulkAnswerModel> raspunsuri) {
            Answers = raspunsuri.ToList();
        }

        public int ObserverId { get; set; }

        public List<BulkAnswerModel> Answers { get; set; }
    }
    public class CompleteazaRaspunsCommand : IRequest<int> {
        public CompleteazaRaspunsCommand() {
            Answers = new List<AnswerDTO>();
        }
        public int ObserverId { get; set; }
        public List<AnswerDTO> Answers { get; set; }

    }
}
