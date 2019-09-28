using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Queries
{
    public class FormVersionQueryHandler : AsyncRequestHandler<FormVersionQuery, List<Entities.Form>>
    {

        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public FormVersionQueryHandler(VoteMonitorContext context, IMapper mapper, ICacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _cacheService = cacheService;
        }
      
        protected override async Task<List<Entities.Form>> HandleCore(FormVersionQuery request)
        {
            var result = await _context.Forms
                .AsNoTracking()
                .ToListAsync();

            return result;
        }
    }
}
