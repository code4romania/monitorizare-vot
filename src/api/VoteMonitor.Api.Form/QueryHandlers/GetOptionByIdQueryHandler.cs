using MediatR;
using VoteMonitor.Api.Form.Models.Options;
using VoteMonitor.Api.Form.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.QueryHandlers;

public class GetOptionByIdQueryHandler : IRequestHandler<GetOptionByIdQuery, OptionDTO>

{
    private readonly VoteMonitorContext _context;

    public GetOptionByIdQueryHandler(VoteMonitorContext context)
    {
        _context = context;
    }

    public Task<OptionDTO> Handle(GetOptionByIdQuery request, CancellationToken cancellationToken)
    {
        var option = _context.Options.FirstOrDefault(c => c.Id == request.OptionId);

        var optionDto = new OptionDTO
        {
            Id = option.Id, Hint = option.Hint, IsFreeText = option.IsFreeText, Text = option.Text
        };

        return Task.FromResult(optionDto);
    }
}
