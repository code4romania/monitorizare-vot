using System.Collections.Generic;
using System.Linq;
using MediatR;
using VoteMonitor.Api.Answer.Models;

namespace VoteMonitor.Api.Answer.Commands
{
    public class BulkAnswers : IRequest<CompleteazaRaspunsCommand> {
        public BulkAnswers(IEnumerable<BulkAnswerModel> raspunsuri) {
            Answers = raspunsuri.ToList();
        }

        public int ObserverId { get; set; }

        public List<BulkAnswerModel> Answers { get; set; }
    }
}