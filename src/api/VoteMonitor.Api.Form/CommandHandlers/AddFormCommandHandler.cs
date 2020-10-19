using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Form.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.CommandHandlers
{
    public class AddFormCommandHandler : IRequestHandler<AddFormCommand, FormDTO>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;

        public AddFormCommandHandler(VoteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<FormDTO> Handle(AddFormCommand message, CancellationToken cancellationToken)
        {
            var newForm = new Entities.Form();
            var formMapper = new FormDbMapper(_context, _mapper);

            formMapper.Map(ref newForm, message.Form);
            _context.Forms.Add(newForm);

            await _context.SaveChangesAsync();
            message.Form.Id = newForm.Id;
            return message.Form;
        }
    }
}