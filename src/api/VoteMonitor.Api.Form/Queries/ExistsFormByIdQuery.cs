using MediatR;

namespace VoteMonitor.Api.Form.Queries
{
    public class GetFormExistsByIdQuery : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
