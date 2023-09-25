using MediatR;

namespace VoteMonitor.Api.Form.Commands;

public record DeleteSectionCommand(int SectionId) : IRequest<bool>;
