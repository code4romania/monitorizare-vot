using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using VoteMonitor.Api.Auth.Models;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Core.Extensions;
using VoteMonitor.Api.Core.Models;
using VoteMonitor.Api.Core.Options;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Auth.Controllers
{
    /// <inheritdoc />
    [Route("api/v2/access")]
    public class AuthorizationV2Controller : AuthorizationControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<AuthorizationV2Controller> _localizer;
        private readonly MobileSecurityOptions _mobileSecurityOptions;
        private readonly JwtIssuerOptions _jwtOptions;

        /// <inheritdoc />
        public AuthorizationV2Controller(IOptions<JwtIssuerOptions> jwtOptions, 
            ILogger<AuthorizationV2Controller> logger, 
            IMediator mediator, 
            IOptions<MobileSecurityOptions> mobileSecurityOptions,
            IStringLocalizer<AuthorizationV2Controller> localizer)
            : base(jwtOptions, mediator, mobileSecurityOptions)
        {
            _logger = logger;
            _mediator = mediator;
            _localizer = localizer;
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

            var identity = await GetClaimsIdentity(request.User, request.Password, request.FcmToken, MobileDeviceIdType.FcmToken);

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
                    Token = request.FcmToken,
                });

                _logger.LogInformation($"Observer {observerId} registered for notifications");
            }

            var token = GetTokenFromIdentity(identity);

            // Serialize and return the response
            var response = new AuthenticationResponseModel
            {
                access_token = token,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };

            return Ok(response);
        }

        /// <summary>
        /// This method is used for test. will be removed after this approch is approved;
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("test-localization")]
        public IActionResult TestLocalization(string name)
        {
            string text = string.Format(_localizer["greeting"].Value, name);
            string noValue = _localizer["no-translation"].Value;
            string onlyEnglish = _localizer["greeting-only-en"].Value;

            return Ok(new { 
                formattedText= text ,
                noValue,
                onlyEnglish
            });
        }
    }
}
