using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Auth.Models;
using VoteMonitor.Api.Auth.Queries;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Auth.Handlers;

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
            .Select(x=> new UserInfo
            {
                IdNgo = x.IdNgo,
                NgoAdminId = x.Id,
                Organizer = x.Ngo.Organizer
            })
            .FirstOrDefault();

        return await Task.FromResult(userinfo);
    }
}
