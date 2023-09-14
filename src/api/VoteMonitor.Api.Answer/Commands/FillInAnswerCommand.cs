using MediatR;
using VoteMonitor.Api.Answer.Models;

namespace VoteMonitor.Api.Answer.Commands;

public record FillInAnswerCommand : IRequest<int>
{
    public FillInAnswerCommand(int observerId, IEnumerable<AnswerDto> answers)
    {
        ObserverId = observerId;
        Answers = answers.ToList().AsReadOnly();
    }

    public int ObserverId { get; }
    public IReadOnlyCollection<AnswerDto> Answers { get; }
}
