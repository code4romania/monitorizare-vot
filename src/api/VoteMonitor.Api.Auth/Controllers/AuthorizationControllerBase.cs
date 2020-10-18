using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using VoteMonitor.Api.Auth.Commands;
using VoteMonitor.Api.Auth.Queries;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Extensions;
using VoteMonitor.Api.Core.Models;
using VoteMonitor.Api.Core.Options;

namespace VoteMonitor.Api.Auth.Controllers
{
    public class AuthorizationControllerBase : Controller
    {
        private readonly IMediator _mediator;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly MobileSecurityOptions _mobileSecurityOptions;

        public AuthorizationControllerBase(IOptions<JwtIssuerOptions> jwtOptions, IMediator mediator, IOptions<MobileSecurityOptions> mobileSecurityOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _mediator = mediator;
            _mobileSecurityOptions = mobileSecurityOptions.Value;
        }

        protected async Task<ClaimsIdentity> GetGenericIdentity(string name, string idNgo, string usertype)
        {
            return new ClaimsIdentity(
                new GenericIdentity(name, ClaimsHelper.GenericIdProvider),
                new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, name),
                    new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                    new Claim(JwtRegisteredClaimNames.Iat,
                        _jwtOptions.IssuedAt.ToUnixEpochDate().ToString(),
                        ClaimValueTypes.Integer64),
                    // Custom
                    new Claim(ClaimsHelper.IdNgo, idNgo),
                    new Claim(ClaimsHelper.UserType, usertype)
                });
        }

        protected string GetTokenFromIdentity(ClaimsIdentity identity)
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

        protected async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password, string uniqueId)
        {
            if (string.IsNullOrEmpty(uniqueId))
            {
                var userInfo = await _mediator.Send(new NgoAdminApplicationUser
                {
                    Password = password,
                    UserName = userName,
                    UserType = UserType.NgoAdmin
                });

                if (userInfo == null)
                {
                    return null;
                }

                // Get the generic claims + the user specific one (the organizer flag)
                return new ClaimsIdentity(await GetGenericIdentity(userName, userInfo.IdNgo.ToString(), UserType.NgoAdmin.ToString()),
                    new[]
                    {
                        new Claim(ClaimsHelper.Organizer, userInfo.Organizer.ToString(), ClaimValueTypes.Boolean),
                        new Claim(ClaimsHelper.NgoAdminIdProperty, userInfo.NgoAdminId.ToString(),  ClaimValueTypes.Integer32)
                    });
            }

            // We check if the user exists and, if it doesn't have another device associated, then it's returned from the database.
            var mobileUserInfo = await _mediator.Send(new ObserverApplicationUser
            {
                Phone = userName,
                Pin = password,
                UDID = uniqueId
            });

            if (!mobileUserInfo.IsAuthenticated)
            {
                return await Task.FromResult<ClaimsIdentity>(null);
            }

            if (mobileUserInfo.FirstAuthentication && _mobileSecurityOptions.LockDevice)
            {
                await
                    _mediator.Send(new RegisterDeviceId
                    {
                        MobileDeviceId = uniqueId,
                        ObserverId = mobileUserInfo.ObserverId
                    });
            }

            // Get the generic claims + the user specific one (the organizer flag)
            return new ClaimsIdentity(await GetGenericIdentity(userName, mobileUserInfo.IdNgo.ToString(), UserType.Observer.ToString()),
                new[]
                {
                        new Claim(ClaimsHelper.ObserverIdProperty, mobileUserInfo.ObserverId.ToString(), ClaimValueTypes.String)
                });
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
    }
}
