using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Form.Models.Options;
using VoteMonitor.Api.Form.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.QueryHandlers;

public class FetchAllOptionsQueryHandler : IRequestHandler<FetchAllOptionsQuery, List<OptionDTO>>
{
    private readonly VoteMonitorContext _context;

    public FetchAllOptionsQueryHandler(VoteMonitorContext context)
    {
        _context = context;
    }

    public async Task<List<OptionDTO>> Handle(FetchAllOptionsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Options.Select(x => new OptionDTO
        {
            Hint = x.Hint,
            Id = x.Id,
            IsFreeText = x.IsFreeText,
            Text = x.Text,
        }).ToListAsync(cancellationToken);
    }
}
