using AutoMapper;
using MediatR;
using VoteMonitor.Api.Form.Models.Options;
using VoteMonitor.Api.Form.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.QueryHandlers;

public class GetOptionByIdQueryHandler : IRequestHandler<GetOptionByIdQuery, OptionDTO>

{
    private readonly VoteMonitorContext _context;
    private readonly IMapper _mapper;

    public GetOptionByIdQueryHandler(VoteMonitorContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<OptionDTO> Handle(GetOptionByIdQuery request, CancellationToken cancellationToken)
    {
        var option = _context.Options.FirstOrDefault(c => c.Id == request.OptionId);

        var optionDto = _mapper.Map<OptionDTO>(option);

        return Task.FromResult(optionDto);
    }
}