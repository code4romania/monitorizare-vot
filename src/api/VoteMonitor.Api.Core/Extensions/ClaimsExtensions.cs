using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace VoteMonitor.Api.Core.Extensions
{
    public static class ClaimsExtensions
    {
        public static int? GetObserverId(this IEnumerable<Claim> claims)
        {
            return int.TryParse(claims.FirstOrDefault(c => c.Type == ClaimsHelper.ObserverIdProperty)?.Value, out var observerId) ? observerId : (int?)null;
        }
    }
}
