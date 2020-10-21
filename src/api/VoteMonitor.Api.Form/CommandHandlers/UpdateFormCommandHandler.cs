using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Api.Form.Mappers;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.CommandHandlers
{
    public class UpdateFormCommandHandler : IRequestHandler<UpdateFormCommand, FormDTO>
    {
        private readonly VoteMonitorContext _context;
        private readonly IEntityMapper<Entities.Form, FormDTO> _entityMapper;

        public UpdateFormCommandHandler(VoteMonitorContext context, IEntityMapper<Entities.Form, FormDTO> updateOrCreateFormMapper)
        {
            _context = context;
            _entityMapper = updateOrCreateFormMapper;
        }

        public async Task<FormDTO> Handle(UpdateFormCommand message, CancellationToken cancellationToken)
        {
            var form = await _context.Forms.Include(f => f.FormSections)
                .ThenInclude(fs => fs.Questions)
                .ThenInclude(q => q.OptionsToQuestions)
                .ThenInclude(otq => otq.Option)
                .FirstOrDefaultAsync(f => f.Id == message.Id);

            _entityMapper.Map(ref form, message.Form);

            await _context.SaveChangesAsync();
            return message.Form;
        }
    }
}