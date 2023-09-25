using MediatR;
using VoteMonitor.Api.Form.Models.Options;

namespace VoteMonitor.Api.Form.Commands;

public record AddOptionCommand(string Text, string Hint, bool IsFreeText) : IRequest<OptionDTO>;
