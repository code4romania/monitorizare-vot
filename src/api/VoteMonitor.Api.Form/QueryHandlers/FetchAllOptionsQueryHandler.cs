using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Models.Options;
using VoteMonitor.Api.Form.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.QueryHandlers
{
    public class FetchAllOptionsQueryHandler : IRequestHandler<FetchAllOptionsQuery, List<OptionDTO>>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;

        public FetchAllOptionsQueryHandler(VoteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<OptionDTO>> Handle(FetchAllOptionsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Options.Select(x => _mapper.Map<OptionDTO>(x)).ToListAsync(cancellationToken);
        }
    }
}
