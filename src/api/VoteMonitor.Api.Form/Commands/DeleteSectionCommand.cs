using MediatR;

namespace VoteMonitor.Api.Form.Commands;

public class DeleteSectionCommand : IRequest<bool>
{
    public int SectionId { get; set; }
}