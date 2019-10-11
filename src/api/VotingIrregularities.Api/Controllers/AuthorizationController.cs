using System;
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
using VotingIrregularities.Api.Models.Authentification;
using VotingIrregularities.Api.Options;
using VotingIrregularities.Api.Queries;
using VotingIrregularities.Domain.UserAggregate;

namespace VotingIrregularities.Api.Controllers
{
	[Route("api/v1/access")]
	public class AuthorizationController : Controller
	{
		private readonly IMediator _mediator;
		private readonly ILogger _logger;
		private readonly JwtIssuerOptions _jwtOptions;
		private readonly MobileSecurityOptions _mobileSecurityOptions;

		public AuthorizationController(IMediator mediator, IOptions<JwtIssuerOptions> jwtOptions, ILogger logger, IOptions<MobileSecurityOptions> mobileSecurityOptions)
		{
			_mediator = mediator;
			_logger = logger;
			_mobileSecurityOptions = mobileSecurityOptions.Value;
			ThrowIfInvalidOptions(jwtOptions.Value);
			_jwtOptions = jwtOptions.Value;
		}

		[HttpPost]
		[AllowAnonymous]
		[ProducesResponseType(typeof(AuthenticationResponseModel), 200)]
		[ProducesResponseType(typeof(void), 400)]
		[ProducesResponseType(typeof(void), 500)]
		public async Task<IActionResult> Login([FromBody] UserLoginDetailsModel loginDetails)
		{
			var ngoQuery = new GetNgoAdminUserInfoQuery(loginDetails.Username, loginDetails.Password);
			var ngoUserInfo = await _mediator.Send(ngoQuery);

			if (ngoUserInfo.IsAuthenticated)
			{
				var ngoUserIdentity = await GetNgoUserIdentityDetails(ngoUserInfo);
				return Ok(GetTokenResponse(ngoUserIdentity));
			}
			_logger.LogInformation($"Invalid username ({ngoQuery.UserName}) or password ({ngoQuery.Password})");

			if (_mobileSecurityOptions.LockDevice && string.IsNullOrEmpty(loginDetails.UDID))
			{
				return BadRequest("User device id nu poate fi gol/null");
			}

			var observerQuery = new GetObserverUserInfoQuery(loginDetails.Username, loginDetails.Password, loginDetails.UDID);
			var observerInfo = await _mediator.Send(observerQuery);

			if (observerInfo.IsAuthenticated)
			{
				if (observerInfo.FirstAuthentication && _mobileSecurityOptions.LockDevice)
				{
					var registerDeviceRequest = new RegisterDeviceId
					{
						MobileDeviceId = observerInfo.UDID,
						ObserverId = observerInfo.ObserverId
					};

					await _mediator.Send(registerDeviceRequest);
				}

				var observerIdentity = await GetObserverIdentityDetails(observerInfo);
				return Ok(GetTokenResponse(observerIdentity));
			}

			_logger.LogInformation($"Invalid username ({observerQuery.UserName}) or password ({observerQuery.Password})");


			return BadRequest("{ \"error\": \"A aparut o eroare la logarea in aplicatie. Va rugam sa verificati ca ati introdus corect numarul de telefon si codul de acces, iar daca eroarea persista va rugam contactati serviciul de suport la numarul 0757652712.\" }");
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

		private async Task<ClaimsIdentity> GetNgoUserIdentityDetails(NgoUserInfo ngoUserInfo)
		{
			return new ClaimsIdentity(await GetGenericIdentity(ngoUserInfo.UserName, ngoUserInfo.IdNgo.ToString(), UserType.NgoAdmin),
				new[]
				{
					new Claim(ClaimsHelper.Organizer, ngoUserInfo.Organizer.ToString(), ClaimValueTypes.Boolean)
				});
		}

		private async Task<ClaimsIdentity> GetObserverIdentityDetails(RegisteredObserverInfo user)
		{
			return new ClaimsIdentity(await GetGenericIdentity(user.Phone, user.IdNgo.ToString(), UserType.Observer),
				new[]
				{
					new Claim(ClaimsHelper.ObserverIdProperty, user.ObserverId.ToString(), ClaimValueTypes.Boolean),
				});
		}

		private async Task<ClaimsIdentity> GetGenericIdentity(string name, string idNgo, UserType userType)
		{
			var issuedAt = ToUnixEpochDate(_jwtOptions.IssuedAt).ToString();

			return new ClaimsIdentity(
				new GenericIdentity(name, ClaimsHelper.GenericIdProvider),
				new[]
				{
					new Claim(JwtRegisteredClaimNames.Sub, name),
					new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
					new Claim(JwtRegisteredClaimNames.Iat,issuedAt, ClaimValueTypes.Integer64),
					// Custom
					new Claim(ClaimsHelper.IdNgo, idNgo),
					new Claim(ClaimsHelper.UserType, userType.ToString())
				});
		}

		private AuthenticationResponseModel GetTokenResponse(ClaimsIdentity identity)
		{
			var token = GetTokenFromIdentity(identity);

			// Serialize and return the response
			var response = new AuthenticationResponseModel
			{
				access_token = token,
				expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
			};

			return response;
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


		/// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
		private static long ToUnixEpochDate(DateTime date)
			=> (long)Math.Round((date.ToUniversalTime() -
								 new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
				.TotalSeconds);

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
	}
}