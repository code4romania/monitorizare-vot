using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Api.Services;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Queries
{
    public class FormVersionQueryHandler : AsyncRequestHandler<FormVersionQuery, Dictionary<string, int>>
    {

        private readonly VotingContext _context;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public FormVersionQueryHandler(VotingContext context, IMapper mapper, ICacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _cacheService = cacheService;
        }
        protected override async Task<Dictionary<string, int>> HandleCore(FormVersionQuery message)
        {
            var result = await _context.FormVersions
                .AsNoTracking()
                .ToListAsync();

            return result.ToDictionary(k => k.Code, v => v.CurrentVersion);
        }
    }
}
