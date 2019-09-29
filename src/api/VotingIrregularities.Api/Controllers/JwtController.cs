using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VoteMonitor.Api.Core;
using VotingIrregularities.Api.Models.AccountViewModels;
using Jwt;

namespace MonitorizareVot.Api.Controllers
{
    [Route("api/v1/auth")]
    public class JwtController : Controller
    {
        private readonly IMediator _mediator;
        private readonly JwtIssuerOptions _jwtOptions;

        private readonly ILogger _logger;

        public JwtController(IOptions<JwtIssuerOptions> jwtOptions, ILoggerFactory loggerFactory, IMediator mediator)
        {
            _mediator = mediator;
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _logger = loggerFactory.CreateLogger<JwtController>();
        }

        [HttpGet]
        [AllowAnonymous]
        // this method will only be called the token is expired
        public async Task<IActionResult> RefreshLogin()
        {
            string token = Request.Headers[ControllerExtensions.AUTH_HEADER_VALUE];
            if (string.IsNullOrEmpty(token))
                return Forbid();
            if (token.StartsWith(ControllerExtensions.BEARER_VALUE, StringComparison.OrdinalIgnoreCase))
                token = token.Substring(ControllerExtensions.BEARER_VALUE.Length).Trim();
            if (string.IsNullOrEmpty(token))
                return Forbid();

            var decoded = JsonWebToken.DecodeToObject<Dictionary<string, string>>(token,
                _jwtOptions.SigningCredentials.Kid, false);
            var idOng = int.Parse(decoded[ControllerExtensions.ID_NGO_VALUE]);
            var organizator = bool.Parse(decoded[ControllerExtensions.ORGANIZER_VALUE]);
            var userName = decoded[JwtRegisteredClaimNames.Sub];

            var json = await GenerateToken(userName, idOng, organizator);

            return new OkObjectResult(json);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] ApplicationUser applicationUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //await _mediator.Send(new ImportObserversRequest {FilePath = "d:\\mv-name-rest.11.15.txt", IdNgo = 3, NameIndexInFile = 2}); // mv
            //await _mediator.Send(new ImportObserversRequest { FilePath = "d:\\dv-rest7.txt", IdNgo = 2, NameIndexInFile = 0 }); // usr

            var identity = await GetClaimsIdentity(applicationUser);
            if (identity == null)
            {
                _logger.LogInformation(
                    $"Invalid username ({applicationUser.UserName}) or password ({applicationUser.Password})");
                return BadRequest("Invalid credentials");
            }
            var json = await GenerateToken(applicationUser.UserName,
                                            int.Parse(identity.Claims.FirstOrDefault(c => c.Type == ControllerExtensions.ID_NGO_VALUE)?.Value),
                                            bool.Parse(identity.Claims.FirstOrDefault(c => c.Type == ControllerExtensions.ORGANIZER_VALUE)?.Value));

            return new OkObjectResult(json);
        }

        [Authorize]
        [HttpPost("test")]
        public async Task<object> Test()
        {
            var claims = User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            });

            return await Task.FromResult(claims);
        }

        private async Task<string> GenerateToken(string userName, int idOng = 0, bool organizator = false)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(),
                    ClaimValueTypes.Integer64),
                new Claim(ControllerExtensions.ID_NGO_VALUE, idOng.ToString()),
                new Claim(ControllerExtensions.AUTH_HEADER_VALUE, organizator.ToString())
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
            //var response = new
            //{
            //    token = encodedJwt,
            //    expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            //};

            //var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return encodedJwt;
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
            var userInfo = await _mediator.Send(user);

            if (userInfo == null)
                return await Task.FromResult<ClaimsIdentity>(null);

            return await Task.FromResult(new ClaimsIdentity(
                new GenericIdentity(user.UserName, ControllerExtensions.TOKEN_VALUE), new[]
                {
                    new Claim(ControllerExtensions.ID_NGO_VALUE, userInfo.IdNgo.ToString()),
                    new Claim(ControllerExtensions.ORGANIZER_VALUE, userInfo.Organizer.ToString(), typeof(bool).ToString())
                }));
        }
    }
}