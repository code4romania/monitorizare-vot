using Microsoft.AspNetCore.Authorization;

namespace VoteMonitor.Api.Core.AuthorizationRequirements
{
    public class IsNonBannedAdminRequirement : IAuthorizationRequirement
    {
        public IsNonBannedAdminRequirement()
        {
        }
    }
}
