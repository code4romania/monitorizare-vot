using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Answer.Models;

namespace VoteMonitor.Api.Answer.Commands
{
    public class FillInAnswerCommand : IRequest<int>
    {
        public FillInAnswerCommand()
        {
            Answers = new List<AnswerDto>();
        }
        public int ObserverId { get; set; }
        public List<AnswerDto> Answers { get; set; }
    }
}