using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Form.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.CommandHandlers
{
    public class UpdateFormCommandHandler : IRequestHandler<UpdateFormCommand, FormDTO>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;

        public UpdateFormCommandHandler(VoteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<FormDTO> Handle(UpdateFormCommand message, CancellationToken cancellationToken)
        {
            var form = await _context.Forms.Include(f => f.FormSections)
                .ThenInclude(fs => fs.Questions)
                .ThenInclude(q => q.OptionsToQuestions)
                .FirstOrDefaultAsync(f => f.Id == message.Id);

            var formUpdater = new FormDbMapper(_context, _mapper);
            formUpdater.Map(ref form, message.Form);

            await _context.SaveChangesAsync();
            return message.Form;
        }
    }
}