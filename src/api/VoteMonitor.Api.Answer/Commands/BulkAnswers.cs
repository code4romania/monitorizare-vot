using MediatR;
using System.Collections.Generic;
using System.Linq;
using VoteMonitor.Api.Answer.Models;

namespace VoteMonitor.Api.Answer.Commands
{
    public class BulkAnswers : IRequest<FillInAnswerCommand>
    {
        public BulkAnswers(IEnumerable<BulkAnswerDto> answers)
        {
            Answers = answers.ToList();
        }

        public int ObserverId { get; set; }

        public List<BulkAnswerDto> Answers { get; set; }
    }
}
