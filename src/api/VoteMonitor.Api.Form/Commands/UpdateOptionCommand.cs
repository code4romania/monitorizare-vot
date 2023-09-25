using MediatR;

namespace VoteMonitor.Api.Form.Commands;

public record UpdateOptionCommand(int OptionId, string Text, string Hint, bool IsFreeText) : IRequest<int>;
