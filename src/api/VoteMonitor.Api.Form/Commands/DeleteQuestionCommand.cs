using MediatR;

namespace VoteMonitor.Api.Form.Commands;

public record DeleteQuestionCommand(int SectionId, int QuestionId) : IRequest<bool>;
