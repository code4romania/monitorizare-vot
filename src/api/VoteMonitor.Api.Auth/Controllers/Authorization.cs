using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using VoteMonitor.Api.Auth.Commands;
using VoteMonitor.Api.Auth.Models;
using VoteMonitor.Api.Auth.Queries;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Models;
using VoteMonitor.Api.Core.Options;

namespace VoteMonitor.Api.Auth.Controllers
{
    /// <inheritdoc />
    [Route("api/v1/access")]
    public class Authorization : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly MobileSecurityOptions _mobileSecurityOptions;

        /// <inheritdoc />
        public Authorization(IOptions<JwtIssuerOptions> jwtOptions, ILogger<Authorization> logger, IMediator mediator, IOptions<MobileSecurityOptions> mobileSecurityOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _logger = logger;
            _mediator = mediator;
            _mobileSecurityOptions = mobileSecurityOptions.Value;
        }

        [HttpPost("authorize")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticationResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AuthenticateUser([FromBody] AuthenticateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string token;
            if (string.IsNullOrEmpty(request.UniqueId))
            {
                var identity = await GetClaimsIdentity(request);
                if (identity == null)
                {
                    _logger.LogInformation($"Invalid username ({request.User}) or password ({request.Password})");
                    return BadRequest("Invalid credentials");
                }

                token = GetTokenFromIdentity(identity);
            }
            else
            {
                var identity = await GetClaimsIdentity(request);

                if (identity == null)
                {
                    _logger.LogInformation($"Invalid Phone ({request.User}) or password ({request.Password})");
                    return BadRequest(_mobileSecurityOptions.InvalidCredentialsErrorMessage);
                }

                token = GetTokenFromIdentity(identity);
            }

            // Serialize and return the response
            var response = new AuthenticationResponseModel
            {
                access_token = token,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };

            return Ok(response);
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
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

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

        private async Task<ClaimsIdentity> GetClaimsIdentity(AuthenticateUserRequest request)
        {
            if (string.IsNullOrEmpty(request.UniqueId))
            {
                var userInfo = await _mediator.Send(new NgoAdminApplicationUser
                {
                    Password = request.Password,
                    UserName = request.User,
                    UserType = UserType.NgoAdmin
                });

                if (userInfo == null)
                {
                    return null;
                }

                // Get the generic claims + the user specific one (the organizer flag)
                return new ClaimsIdentity(await GetGenericIdentity(request.User, userInfo.IdNgo.ToString(), UserType.NgoAdmin.ToString()),
                    new[]
                    {
                        new Claim(ClaimsHelper.Organizer, userInfo.Organizer.ToString(), ClaimValueTypes.Boolean),
                        new Claim(ClaimsHelper.NgoAdminIdProperty, userInfo.NgoAdminId.ToString(),  ClaimValueTypes.Integer32)
                    });
            }
            else
            {
                // verific daca userul exista si daca nu are asociat un alt device, il returneaza din baza
                var userInfo = await _mediator.Send(new ObserverApplicationUser
                {
                    Phone = request.User,
                    Pin = request.Password,
                    UDID = request.UniqueId
                });

                if (!userInfo.IsAuthenticated)
                {
                    return await Task.FromResult<ClaimsIdentity>(null);
                }

                if (userInfo.FirstAuthentication && _mobileSecurityOptions.LockDevice)
                {
                    await
                        _mediator.Send(new RegisterDeviceId
                        {
                            MobileDeviceId = request.UniqueId,
                            ObserverId = userInfo.ObserverId
                        });
                }

                // Get the generic claims + the user specific one (the organizer flag)
                return new ClaimsIdentity(await GetGenericIdentity(request.User, userInfo.IdNgo.ToString(), UserType.Observer.ToString()),
                    new[]
                    {
                        new Claim(ClaimsHelper.ObserverIdProperty, userInfo.ObserverId.ToString(), ClaimValueTypes.String)
                    });
            }

        }
        private string GetTokenFromIdentity(ClaimsIdentity identity)
        {
            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: identity.Claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
        private async Task<ClaimsIdentity> GetGenericIdentity(string name, string idNgo, string usertype)
        {
            return new ClaimsIdentity(
                new GenericIdentity(name, ClaimsHelper.GenericIdProvider),
                new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, name),
                    new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                    new Claim(JwtRegisteredClaimNames.Iat,
                        ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(),
                        ClaimValueTypes.Integer64),
                    // Custom
                    new Claim(ClaimsHelper.IdNgo, idNgo),
                    new Claim(ClaimsHelper.UserType, usertype)
                });
        }
    }
}
