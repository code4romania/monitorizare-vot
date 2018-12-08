using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MediatR;
using VotingIrregularities.Domain.AnswerAggregate.Commands;

namespace VotingIrregularities.Api.Models
{
    public class AnswerModelWrapper
    {
        public AnswerModelBulk[] Answer { get; set; }
    }

    public class AnswerModelBulk
    {
        [Required]
        public int QuestionId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CountyCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int SectionNumber { get; set; }

        //[Required(AllowEmptyStrings = false)]
        public string FormCode { get; set; }
        public List<SelectedOptionsModel> Options { get; set; }
    }

    public class AnswersBulk : IRequest<SendAnswerCommand>
    {
        public AnswersBulk(IEnumerable<AnswerModelBulk> answers)
        {
            AnswersModelBulk = answers.ToList();
        }

        public int ObserverId { get; set; }

        public List<AnswerModelBulk> AnswersModelBulk { get; set; }
    }
}
