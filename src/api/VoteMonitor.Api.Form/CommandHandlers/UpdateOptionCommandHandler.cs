using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.CommandHandlers;

public class UpdateOptionCommandHandler : IRequestHandler<UpdateOptionCommand, int>
{
    private readonly VoteMonitorContext _context;
    private readonly IMapper _mapper;

    public UpdateOptionCommandHandler(VoteMonitorContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(UpdateOptionCommand request, CancellationToken cancellationToken)
    {
        var option = await _context.Options
            .FirstOrDefaultAsync(a => a.Id == request.Option.Id, cancellationToken);

        if (option == null)
        {
            throw new ArgumentException($"Can't find this option by id = {request.Option.Id}");
        }

        _mapper.Map(request.Option, option);
        _context.Update(option);

        return await _context.SaveChangesAsync(cancellationToken);
    }
}