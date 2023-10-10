using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Api.Form.Mappers;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.CommandHandlers;

public class UpdateFormCommandHandler : IRequestHandler<UpdateFormCommand, FormDTO>
{
    private readonly VoteMonitorContext _context;
    private readonly IEntityMapper<Entities.Form, FormDTO> _entityMapper;
    private readonly ICacheService _cacheService;

    public UpdateFormCommandHandler(VoteMonitorContext context,
        IEntityMapper<Entities.Form, FormDTO> updateOrCreateFormMapper,
        ICacheService cacheService)
    {
        _context = context;
        _entityMapper = updateOrCreateFormMapper;
        _cacheService = cacheService;
    }

    public async Task<FormDTO> Handle(UpdateFormCommand message, CancellationToken cancellationToken)
    {
        var form = await _context.Forms.Include(f => f.FormSections)
            .ThenInclude(fs => fs.Questions)
            .ThenInclude(q => q.OptionsToQuestions)
            .ThenInclude(otq => otq.Option)
            .Where(f => f.Id == message.Id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        _entityMapper.Map(ref form, message.Form);

        form!.CurrentVersion += 1;
        await _context.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveValueAsync($"Formular{form.Code}");
        return message.Form;
    }
}
