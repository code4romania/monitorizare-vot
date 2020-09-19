using MediatR;

namespace VoteMonitor.Api.Form.Queries
{
    public class DeleteFormCommand : IRequest<bool>
    {
        public int FormId { get; set; }
    }
}
