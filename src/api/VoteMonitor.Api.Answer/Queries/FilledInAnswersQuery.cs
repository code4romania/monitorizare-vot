using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Answer.Models;

namespace VoteMonitor.Api.Answer.Queries {
    public class FilledInAnswersQuery : IRequest<List<QuestionDTO<FilledInAnswerDTO>>> {
        public int PollingStationId { get; set; }
        public int ObserverId { get; set; }
    }
}