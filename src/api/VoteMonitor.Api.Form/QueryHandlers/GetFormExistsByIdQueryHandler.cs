using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.QueryHandlers
{
    public class GetFormExistsByIdQueryHandler : IRequestHandler<GetFormExistsByIdQuery, bool>
    {
        private readonly VoteMonitorContext _context;

        public GetFormExistsByIdQueryHandler(VoteMonitorContext voteMonitorContext)
        {
            _context = voteMonitorContext;
        }

        public Task<bool> Handle(GetFormExistsByIdQuery request, CancellationToken cancellationToken)
        {
            return _context.Forms.AnyAsync(form => form.Id == request.Id, cancellationToken);
        }
    }
}
