using MediatR;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Api.Form.Mappers;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.CommandHandlers
{
    public class AddFormCommandHandler : IRequestHandler<AddFormCommand, FormDTO>
    {
        private readonly VoteMonitorContext _context;
        private readonly IFormMapper _formMapper;

        public AddFormCommandHandler(VoteMonitorContext context, IFormMapper formMapper)
        {
            _context = context;
            _formMapper = formMapper;
        }

        public async Task<FormDTO> Handle(AddFormCommand message, CancellationToken cancellationToken)
        {
            Entities.Form form = null;
            _formMapper.Map(ref form, message.Form);

            _context.Forms.Add(form);

            await _context.SaveChangesAsync();
            message.Form.Id = form.Id;
            return message.Form;
        }
    }
}