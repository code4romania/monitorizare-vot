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
using VotingIrregularities.Api.Services;
using VotingIrregularities.Domain.UserAggregate;

namespace VotingIrregularities.Api.Controllers
{
    [Route("api/v1/access")]
    public class Authorization : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly JsonSerializerSettings _serializerSettings;

        public Authorization(IOptions<JwtIssuerOptions> jwtOptions, ILogger logger, IMediator mediator)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _logger = logger;
            _mediator = mediator;

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [HttpPost("token")]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromBody] ApplicationUser applicationUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var identity = await GetClaimsIdentity(applicationUser);
            if (identity == null)
            {
                _logger.LogInformation($"Invalid Phone ({applicationUser.Phone}) or password ({applicationUser.Pin})");
               return BadRequest("{ \"error\": \"La ora asta observatorii ar trebui sa doarma! :) Aplicatia va fi functionala la ora 6. Asigura-te ca ai cea mai recenta versiune. Fa un update!\" }");
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

        private async Task<ClaimsIdentity> GetClaimsIdentity(ApplicationUser user)
        {
            // verific daca userul exista si daca nu are asociat un alt device, il returneaza din baza
            var userInfo = await _mediator.Send(user);

            if (!userInfo.EsteAutentificat)
                return await Task.FromResult<ClaimsIdentity>(null);

            if (userInfo.PrimaAutentificare)
                await
                    _mediator.Send(new InregistreazaDispozitivCommand
                    {
                        IdDispozitivMobil = user.UDID,
                        IdObservator = userInfo.IdObservator
                    });

            return await Task.FromResult(new ClaimsIdentity(
                new GenericIdentity(user.Phone, "Token"),
                new[]
                {
                    new Claim("Observator", "ONG"),
                    new Claim("IdObservator", userInfo.IdObservator.ToString())
                }));
        }
    }
}