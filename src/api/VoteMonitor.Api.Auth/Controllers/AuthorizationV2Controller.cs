using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using VoteMonitor.Api.Auth.Models;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Core.Extensions;
using VoteMonitor.Api.Core.Models;
using VoteMonitor.Api.Core.Options;

namespace VoteMonitor.Api.Auth.Controllers
{
    /// <inheritdoc />
    [Route("api/v2/access")]
    public class AuthorizationV2Controller : AuthorizationControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly MobileSecurityOptions _mobileSecurityOptions;
        private readonly JwtIssuerOptions _jwtOptions;

        /// <inheritdoc />
        public AuthorizationV2Controller(IOptions<JwtIssuerOptions> jwtOptions, ILogger<AuthorizationV2Controller> logger, IMediator mediator, IOptions<MobileSecurityOptions> mobileSecurityOptions)
            : base(jwtOptions, mediator, mobileSecurityOptions)
        {
            _logger = logger;
            _mediator = mediator;
            _mobileSecurityOptions = mobileSecurityOptions.Value;
            _jwtOptions = jwtOptions.Value;
        }


        [HttpPost("authorize")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticationResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AuthenticateUser([FromBody] AuthenticateUserRequestV2 request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string token;
            var identity = await GetClaimsIdentity(request.User, request.Password, request.FcmToken);

            var haveFcmToken = !string.IsNullOrEmpty(request.FcmToken);

            if (identity == null)
            {
                _logger.LogInformation($"Invalid {(haveFcmToken ? "Phone" : "UserName")} ({request.User}) or password");

                return BadRequest(haveFcmToken
                    ? _mobileSecurityOptions.InvalidCredentialsErrorMessage
                    : "Invalid credentials");
            }

            var observerId = identity.Claims.GetObserverId();

            if (haveFcmToken && observerId.HasValue)
            {
                await _mediator.Send(new NotificationRegistrationDataCommand()
                {
                    ChannelName = request.ChannelName,
                    ObserverId = observerId.Value,
                    Token = request.FcmToken
                }
                );

                _logger.LogInformation($"Observer {observerId} registered for notifications");
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
    }
}
