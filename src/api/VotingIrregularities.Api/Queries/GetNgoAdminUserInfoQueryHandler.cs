using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Entities;
using VotingIrregularities.Api.Models.AccountViewModels;

namespace VotingIrregularities.Api.Queries
{
	public class GetNgoAdminUserInfoQueryHandler : AsyncRequestHandler<GetNgoAdminUserInfoQuery, NgoUserInfo>
	{
		private readonly VoteMonitorContext _context;
		private readonly IHashService _hash;

		public GetNgoAdminUserInfoQueryHandler(VoteMonitorContext context, IHashService hash)
		{
			_context = context;
			_hash = hash;
		}

		protected override async Task<NgoUserInfo> HandleCore(GetNgoAdminUserInfoQuery message)
		{
			var hashValue = _hash.GetHash(message.Password);

			var userInfo = _context.NgoAdmins
				.Include(a => a.Ngo)
				.Where(a => a.Password == hashValue && a.Account == message.UserName)
									 .AsEnumerable()
									 .Select(Mapper.Map<NgoUserInfo>)
									 .FirstOrDefault();

			if (userInfo == null)
			{
				userInfo =  new NgoUserInfo()
				{
					IsAuthenticated = false
				};
			}
			else
			{
				userInfo.IsAuthenticated = true;
			}

			return await Task.FromResult(userInfo);
		}
	}
}
