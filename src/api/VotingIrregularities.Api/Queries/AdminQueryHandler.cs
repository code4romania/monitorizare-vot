using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MonitorizareVot.Ong.Api.Models;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Entities;
using VotingIrregularities.Api.Models.AccountViewModels;
using VoteMonitor.Api.Core.Services;

namespace MonitorizareVot.Ong.Api.Queries
{
    public class AdminQueryHandler : IRequestHandler<NgoAdminApplicationUser, UserInfo>
    {
        private readonly VoteMonitorContext _context;
        private readonly IHashService _hash;

        public AdminQueryHandler(VoteMonitorContext context, IHashService hash)
        {
            _context = context;
            _hash = hash;
        }

        public async Task<UserInfo> Handle(NgoAdminApplicationUser message, CancellationToken token)
        {
            var hashValue = _hash.GetHash(message.Password);

            var userinfo = _context.NgoAdmins
                .Include(a => a.Ngo)
                .Where(a => a.Password == hashValue &&
                                     a.Account == message.UserName)
                                     .Select(Mapper.Map<UserInfo>)
                                     .FirstOrDefault();

            return await Task.FromResult(userinfo);
        }
    }
}
