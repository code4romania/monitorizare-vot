using MediatR;

namespace VoteMonitor.Api.Form.Commands
{
    public class DeleteFormCommand : IRequest<bool>
    {
        public int FormId { get; set; }
    }
}
