using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Form.Models.Options;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.CommandHandlers
{
    public class AddOptionCommandHandler : IRequestHandler<AddOptionCommand, OptionDTO>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;

        public AddOptionCommandHandler(VoteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OptionDTO> Handle(AddOptionCommand request, CancellationToken cancellationToken)
        {
            var optionEntity = _mapper.Map<Option>(request.Option);

            _context.Options.Add(optionEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<OptionDTO>(optionEntity);
        }
    }
}
