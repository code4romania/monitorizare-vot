using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Options;
using System;
using VoteMonitor.Api.Core.Models;
using System.Linq;

namespace VoteMonitor.Api.Core.AuthorizationRequirements
{
    public class IsNonBannedAuthorizationHandler : AuthorizationHandler<IsNonBannedAdminRequirement>
    {
        public IsNonBannedAuthorizationHandler(IOptions<AdminBanOptions> options)
        {
            _adminBanOptions = options.Value ?? throw new ArgumentNullException($"Options of type {nameof(AdminBanOptions)} are null");
        }

        private readonly AdminBanOptions _adminBanOptions;

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsNonBannedAdminRequirement requirement)
        {
            if (!_adminBanOptions.IsEnabled)
            {
                context.Succeed(requirement);
                return Task.FromResult(0);
            }

            if (context.User.HasClaim(ClaimsHelper.UserType, UserType.NgoAdmin.ToString()))
            {
                var adminId = int.Parse(context.User.Claims.First(c => c.Type == ClaimsHelper.NgoAdminIdProperty).Value);
                if (_adminBanOptions.BannedIds.Contains(adminId))
                {
                    context.Fail();
                }
            }

            context.Succeed(requirement);
            return Task.FromResult(0);
        }
    }
}
