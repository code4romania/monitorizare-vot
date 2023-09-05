using MediatR;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Api.Form.Mappers;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.CommandHandlers;

public class AddFormCommandHandler : IRequestHandler<AddFormCommand, FormDTO>
{
    private readonly VoteMonitorContext _context;
    private readonly IEntityMapper<Entities.Form, FormDTO> _entityMapper;

    public AddFormCommandHandler(VoteMonitorContext context, IEntityMapper<Entities.Form, FormDTO> entityMapper)
    {
        _context = context;
        _entityMapper = entityMapper;
    }

    public async Task<FormDTO> Handle(AddFormCommand message, CancellationToken cancellationToken)
    {
        Entities.Form form = null;
        _entityMapper.Map(ref form, message.Form);

        _context.Forms.Add(form);

        await _context.SaveChangesAsync();
        message.Form.Id = form.Id;
        return message.Form;
    }
}