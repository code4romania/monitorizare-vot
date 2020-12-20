using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace VoteMonitor.Api.IntegrationTests.Setup
{
    public static class Jwt
    {
        private static readonly IDictionary<Policy, Claim[]> ClaimsForPolicies = new Dictionary<Policy, Claim[]>
        {
            {Policy.NgoAdmin, new[]{new Claim("UserType", "NgoAdmin"), new Claim("IdNgo", "1")}},
            {Policy.Observer, new[]{new Claim("UserType", "Observer"), new Claim("ObserverId", "0")}},
            {Policy.Organizer, new[]{new Claim("Organizer", "true")}}
        };

        public static string CreateJwt(Policy policy, params (string type, string value)[] otherClaims)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("super-signing-secret"));
            var now = DateTime.UtcNow;
            var baseClaims = new[] {
                new Claim(JwtRegisteredClaimNames.Iss, "VoteMonitorJwtIssuer"),
                new Claim(JwtRegisteredClaimNames.Sub, "integration-test"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, "http://localhost:53413/")
            };
            var userTypeClaims = ClaimsForPolicies[policy];
            var claims = baseClaims.Concat(userTypeClaims).ToList();

            if (otherClaims != null)
            {
                claims.RemoveAll(c => otherClaims.Any(o => o.type == c.Type));
                claims = claims.Concat(otherClaims.Select(c1 => new Claim(c1.type, c1.value))).ToList();
            }

            var handler = new JwtSecurityTokenHandler();

            var token = new JwtSecurityToken
            (
                claims: claims,
                notBefore: now.AddMilliseconds(-30),
                expires: now.AddMinutes(60),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return handler.WriteToken(token);
        }
    }
}