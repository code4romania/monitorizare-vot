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
using VoteMonitor.Api.Core;
using VotingIrregularities.Domain.UserAggregate;
using VotingIrregularities.Api.Options;

namespace VotingIrregularities.Api.Controllers
{
    /// <inheritdoc />
    [Route("api/v1/access")]
    public class Authorization : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly MobileSecurityOptions _mobileSecurityOptions;

        /// <inheritdoc />
        public Authorization(IOptions<JwtIssuerOptions> jwtOptions, ILogger logger, IMediator mediator, IOptions<MobileSecurityOptions> mobileSecurityOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _logger = logger;
            _mediator = mediator;
            _mobileSecurityOptions = mobileSecurityOptions.Value;

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        /// <summary>
        /// Get the auth token to be passed to subsequent requests
        /// </summary>
        /// <param name="applicationUser"></param>
        /// <returns></returns>
        [HttpPost("token")]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromBody] ObserverApplicationUser applicationUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var identity = await GetClaimsIdentity(applicationUser);

            if (identity == null)
            {
                _logger.LogInformation($"Invalid Phone ({applicationUser.Phone}) or password ({applicationUser.Pin})");
               return BadRequest("{ \"error\": \"A aparut o eroare la logarea in aplicatie. Va rugam sa verificati ca ati introdus corect numarul de telefon si codul de acces, iar daca eroarea persista va rugam contactati serviciul de suport la numarul 0757652712.\" }");
            }

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Phone),
        new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
        new Claim(JwtRegisteredClaimNames.Iat,
                  ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(),
                  ClaimValueTypes.Integer64),
        identity.FindFirst(ClaimsHelper.ObserverIdProperty)
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

            return Ok(response);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] NgoAdminApplicationUser ngoAdminApplicationUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //await _mediator.Send(new ImportObserversRequest {FilePath = "d:\\mv-name-rest.11.15.txt", IdNgo = 3, NameIndexInFile = 2}); // mv
            //await _mediator.Send(new ImportObserversRequest { FilePath = "d:\\dv-rest7.txt", IdNgo = 2, NameIndexInFile = 0 }); // usr

            var identity = await GetClaimsIdentity(ngoAdminApplicationUser);
            if (identity == null)
            {
                _logger.LogInformation(
                    $"Invalid username ({ngoAdminApplicationUser.UserName}) or password ({ngoAdminApplicationUser.Password})");
                return BadRequest("Invalid credentials");
            }
            var json = await GenerateToken(ngoAdminApplicationUser.UserName,
                int.Parse(identity.Claims.FirstOrDefault(c => c.Type == ClaimsHelper.ID_NGO_VALUE)?.Value),
                bool.Parse(identity.Claims.FirstOrDefault(c => c.Type == ClaimsHelper.ORGANIZER_VALUE)?.Value));

            return new OkObjectResult(json);
        }

        /// <summary>
        /// Test action to get claims
        /// </summary>
        /// <returns></returns>
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
        private async Task<string> GenerateToken(string userName, int idOng = 0, bool organizator = false)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(),
                    ClaimValueTypes.Integer64),
                new Claim(ClaimsHelper.ID_NGO_VALUE, idOng.ToString()),
                new Claim(ClaimsHelper.AUTH_HEADER_VALUE, organizator.ToString())
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
        private async Task<ClaimsIdentity> GetClaimsIdentity(ObserverApplicationUser user)
        {
            // verific daca userul exista si daca nu are asociat un alt device, il returneaza din baza
            var userInfo = await _mediator.Send(user);

            if (!userInfo.IsAuthenticated)
                return await Task.FromResult<ClaimsIdentity>(null);

            if (userInfo.FirstAuthentication && _mobileSecurityOptions.LockDevice)
                await
                    _mediator.Send(new RegisterDeviceId
                    {
                        MobileDeviceId = user.UDID,
                        ObserverId = userInfo.ObserverId
                    });

            return await Task.FromResult(new ClaimsIdentity(
                new GenericIdentity(user.Phone, ClaimsHelper.GenericIdProvider),
                new[]
                {
                    new Claim(ClaimsHelper.ObserverProperty, ClaimsHelper.ObserverDefault),
                    new Claim(ClaimsHelper.ObserverIdProperty, userInfo.ObserverId.ToString())
                }));
        }
        private async Task<ClaimsIdentity> GetClaimsIdentity(NgoAdminApplicationUser user)
        {
            var userInfo = await _mediator.Send(user);

            if (userInfo == null)
                return await Task.FromResult<ClaimsIdentity>(null);

            return await Task.FromResult(new ClaimsIdentity(
                new GenericIdentity(user.UserName, ClaimsHelper.TOKEN_VALUE), new[]
                {
                    new Claim(ClaimsHelper.ID_NGO_VALUE, userInfo.IdNgo.ToString()),
                    new Claim(ClaimsHelper.ORGANIZER_VALUE, userInfo.Organizer.ToString(), typeof(bool).ToString())
                }));
        }
    }
}