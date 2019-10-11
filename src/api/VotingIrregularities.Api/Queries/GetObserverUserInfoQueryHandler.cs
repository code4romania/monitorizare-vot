using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Services;
using VotingIrregularities.Api.Models.AccountViewModels;
using VotingIrregularities.Api.Options;
using VoteMonitor.Entities;

namespace VotingIrregularities.Api.Queries
{
	/// <summary>
	/// Handles the query regarding the authentication of the observer - checks the phone number and hashed pin against the database
	/// </summary>
	public class GetObserverUserInfoQueryHandler : AsyncRequestHandler<GetObserverUserInfoQuery, RegisteredObserverInfo>
	{
		private readonly VoteMonitorContext _context;
		private readonly IHashService _hash;
		private readonly MobileSecurityOptions _mobileSecurityOptions;

		/// <summary>
		/// Constructor for dependency injection
		/// </summary>
		/// <param name="context">The EntityFramework context</param>
		/// <param name="hash">Implementation of the IHashService to be used to generate the hashes. It can either be `HashService` or `ClearTextService`.</param>
		/// <param name="mobileSecurityOptions">Options for specifying the DeviceLock feature toggle. If MobileSecurity:LockDevice is enabled (set to `true` in settings), the first login of the observer will store the UniqueDeviceId and only that device will be allowed to login.</param>
		public GetObserverUserInfoQueryHandler(VoteMonitorContext context, IHashService hash, IOptions<MobileSecurityOptions> mobileSecurityOptions)
		{
			_context = context;
			_hash = hash;
			_mobileSecurityOptions = mobileSecurityOptions.Value;
		}

		protected override async Task<RegisteredObserverInfo> HandleCore(GetObserverUserInfoQuery message)
		{
			var hashValue = _hash.GetHash(message.Password);

			// Check for username and hash
			var userQuery = _context.Observers.Where(o => o.Pin == hashValue && o.Phone == message.UserName);

			// Only if device lock is enabled verify the DeviceId
			if (_mobileSecurityOptions.LockDevice)
				userQuery = userQuery.Where(o => string.IsNullOrWhiteSpace(o.MobileDeviceId) || o.MobileDeviceId == message.UDID);

			var userinfo = await userQuery.FirstOrDefaultAsync();

			if (userinfo == null)
				return new RegisteredObserverInfo
				{
					IsAuthenticated = false
				};

			return new RegisteredObserverInfo
			{
				ObserverId = userinfo.Id,
				IdNgo = userinfo.IdNgo,
				IsAuthenticated = true,
				FirstAuthentication = string.IsNullOrWhiteSpace(userinfo.MobileDeviceId) && _mobileSecurityOptions.LockDevice,
				UDID = message.UDID,
				Phone = userinfo.Phone
			};
		}
	}
}
