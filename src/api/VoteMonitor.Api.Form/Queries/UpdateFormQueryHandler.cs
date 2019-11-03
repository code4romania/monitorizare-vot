using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Queries
{
    public class UpdateFormQueryHandler :
        AsyncRequestHandler<UpdateFormQuery, FormDTO>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;

        public UpdateFormQueryHandler(VoteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        protected override async Task<FormDTO> HandleCore(UpdateFormQuery message)
        {
            var form = _context.Forms.Find(message.Id);
            var formUpdater = new FormDbMapper(_context, _mapper);
            formUpdater.Map(ref form, message.Form);

            await _context.SaveChangesAsync();
            return message.Form;
        }
    }
}