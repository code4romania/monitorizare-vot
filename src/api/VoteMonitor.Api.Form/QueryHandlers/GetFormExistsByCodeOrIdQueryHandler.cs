using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Form.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.QueryHandlers;

public class GetFormExistsByCodeOrIdQueryHandler : IRequestHandler<ExistsFormByCodeOrIdQuery, bool>
{
    private readonly VoteMonitorContext _context;
    public GetFormExistsByCodeOrIdQueryHandler(VoteMonitorContext voteMonitorContext)
    {
        _context = voteMonitorContext;
    }

    public async Task<bool> Handle(ExistsFormByCodeOrIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Forms.AnyAsync(form => form.Code == request.Code || form.Id == request.Id);
    }
}