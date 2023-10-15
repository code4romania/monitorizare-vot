using MediatR;
using VoteMonitor.Api.Answer.Models;

namespace VoteMonitor.Api.Answer.Commands;

public record FillInAnswerCommand : IRequest<int>
{
    public FillInAnswerCommand(int observerId, IEnumerable<AnswerDto> answers, IEnumerable<CorruptedAnswerDto> corruptedAnswers)
    {
        ObserverId = observerId;
        Answers = answers.ToList().AsReadOnly();
        CorruptedAnswers = corruptedAnswers.ToList().AsReadOnly();
    }

    public int ObserverId { get; }
    public IReadOnlyCollection<AnswerDto> Answers { get; }
    public IReadOnlyCollection<CorruptedAnswerDto> CorruptedAnswers { get; }
}
