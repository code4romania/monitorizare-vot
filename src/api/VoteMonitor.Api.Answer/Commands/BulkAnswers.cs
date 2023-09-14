using MediatR;
using VoteMonitor.Api.Answer.Models;

namespace VoteMonitor.Api.Answer.Commands;

public record BulkAnswers : IRequest<FillInAnswerCommand>
{
    public BulkAnswers(int observerId, IEnumerable<BulkAnswerDto> answers)
    {
        ObserverId = observerId;
        Answers = answers.ToList().AsReadOnly();
    }

    public int ObserverId { get; }

    public IReadOnlyCollection<BulkAnswerDto> Answers { get; }
}
