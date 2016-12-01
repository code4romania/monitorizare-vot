using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VotingIrregularities.Api.Models.AccountViewModels;
using System.Linq;
using MediatR;

namespace VotingIrregularities.Api.Controllers
{
    [Route("api/v1/access")]
    public class Authorization : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;

        public Authorization(IOptions<JwtIssuerOptions> jwtOptions, ILogger logger, IMediator mediator)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _logger = logger;

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [HttpPost("token")]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromBody] ApplicationUser applicationUser)
        {
            var identity = await GetClaimsIdentity(applicationUser);
            if (identity == null)
            {
                _logger.LogInformation($"Invalid Phone ({applicationUser.Phone}) or password ({applicationUser.Pin})");
                return BadRequest("Invalid credentials");
            }

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Phone),
        new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
        new Claim(JwtRegisteredClaimNames.Iat,
                  ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(),
                  ClaimValueTypes.Integer64),
        identity.FindFirst("IdObservator")
      };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Serialize and return the response
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }

        [Authorize]
        [HttpPost("test")]
        public async Task<object> Test()
        {
            var claims = User.Claims.Select(c => new
            {
                Type = c.Type,
                Value = c.Value
            });

            return await Task.FromResult(claims);
        }
        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        /// <summary>
        /// IMAGINE BIG RED WARNING SIGNS HERE!
        /// You'd want to retrieve claims through your claims provider
        /// in whatever way suits you, the below is purely for demo purposes!
        /// </summary>
        private static Task<ClaimsIdentity> GetClaimsIdentity(ApplicationUser user)
        {
            if (user.Phone == "0733450005" &&
                user.Pin == "1234" &&
                user.UDID == "UDID")
            {
                return Task.FromResult(new ClaimsIdentity(
                  new GenericIdentity(user.Phone, "Token"),
                  new[]
                  {
            new Claim("Observator", "ONG"),
            new Claim("IdObservator", "1"),
            new Claim(ClaimTypes.Dsa, user.UDID)}));
            }

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }
    }
}