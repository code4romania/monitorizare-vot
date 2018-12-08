using System.Collections.Generic;
using MediatR;

namespace VotingIrregularities.Domain.AnswerAggregate.Commands
{
    public class SendAnswerCommand : IRequest<int>
    {
        public SendAnswerCommand()
        {
            Answers = new List<AnswerModel>();
        }

        public int ObserverId { get; set; }
        public List<AnswerModel> Answers { get; set; }
    }
}
