using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using VoteMonitor.Api.Auth.Models;
using VoteMonitor.Api.Core.Models;
using VoteMonitor.Api.Core.Options;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Auth.Controllers
{
    /// <inheritdoc />
    [Route("api/v1/access")]
    public class AuthorizationV1Controller : AuthorizationControllerBase
    {
        private readonly ILogger _logger;
        private readonly MobileSecurityOptions _mobileSecurityOptions;
        private readonly JwtIssuerOptions _jwtOptions;

        /// <inheritdoc />
        public AuthorizationV1Controller(IOptions<JwtIssuerOptions> jwtOptions, ILogger<AuthorizationV1Controller> logger, IMediator mediator, IOptions<MobileSecurityOptions> mobileSecurityOptions)
            : base(jwtOptions, mediator, mobileSecurityOptions)
        {
            _logger = logger;
            _mobileSecurityOptions = mobileSecurityOptions.Value;
            _jwtOptions = jwtOptions.Value;
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
            var identity = await GetClaimsIdentity(request.User, request.Password, request.UniqueId, MobileDeviceIdType.UserGeneratedGuid);

            var haveUniqueId = !string.IsNullOrEmpty(request.UniqueId);

            if (identity == null)
            {
                _logger.LogInformation($"Invalid {(haveUniqueId ? "Phone" : "UserName")} ({request.User}) or password");

                return BadRequest(haveUniqueId
                    ? _mobileSecurityOptions.InvalidCredentialsErrorMessage
                    : "Invalid credentials");
            }

            token = GetTokenFromIdentity(identity);

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
    }
}
