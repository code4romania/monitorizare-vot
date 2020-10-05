using AutoMapper;
using MediatR;

using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Queries
{
    public class UpdateFormQueryHandler :
        IRequestHandler<UpdateFormQuery, FormDTO>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;

        public UpdateFormQueryHandler(VoteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<FormDTO> Handle(UpdateFormQuery message, CancellationToken cancellationToken)
        {
            var form = _context.Forms.Find(message.Id);
            var formUpdater = new FormDbMapper(_context, _mapper);
            formUpdater.Map(ref form, message.Form);

            await _context.SaveChangesAsync();
            return message.Form;
        }
    }
}