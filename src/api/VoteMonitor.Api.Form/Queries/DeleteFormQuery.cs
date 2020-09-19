using MediatR;

namespace VoteMonitor.Api.Form.Queries
{
    public class DeleteFormModel
    {
        public int FormId { get; set; }
    }

    public class DeleteFormCommand : IRequest<bool>
    {
        public int FormId { get; set; }
    }
}
