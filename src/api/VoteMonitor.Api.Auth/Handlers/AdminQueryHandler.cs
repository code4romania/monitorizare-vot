using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Auth.Models;
using VoteMonitor.Api.Auth.Queries;
using VoteMonitor.Api.HashingService;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Auth.Handlers
{
    public class AdminQueryHandler : IRequestHandler<NgoAdminApplicationUser, UserInfo>
    {
        private readonly VoteMonitorContext _context;
        private readonly IHashService _hash;
        private readonly IMapper _mapper;

        public AdminQueryHandler(VoteMonitorContext context, IHashService hash, IMapper mapper)
        {
            _context = context;
            _hash = hash;
            _mapper = mapper;
        }

        public async Task<UserInfo> Handle(NgoAdminApplicationUser message, CancellationToken token)
        {
            var hashValue = _hash.GetHash(message.Password);

            var userinfo = _context.NgoAdmins
                .Include(a => a.Ngo)
                .Where(a => a.Password == hashValue &&
                                     a.Account == message.UserName)
                .Select(_mapper.Map<UserInfo>)
                .FirstOrDefault();

            return await Task.FromResult(userinfo);
        }
    }
}
