using System.Collections.Generic;
using MediatR;
using VoteMonitor.Api.Answer.Models;

namespace VoteMonitor.Api.Answer.Commands
{
    public class CompleteazaRaspunsCommand : IRequest<int> {
        public CompleteazaRaspunsCommand() {
            Answers = new List<AnswerDTO>();
        }
        public int ObserverId { get; set; }
        public List<AnswerDTO> Answers { get; set; }

    }
}