using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VoteMonitor.Api.Auth.Models;
using VoteMonitor.Api.Auth.Queries;
using VoteMonitor.Api.Core.Options;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Auth.Handlers;

/// <summary>
/// Handles the query regarding the authentication of the observer - checks the phone number and hashed pin against the database
/// </summary>
public class ObserverAuthenticationQueryHandler : IRequestHandler<ObserverApplicationUser, RegisteredObserverModel>
{
    private readonly VoteMonitorContext _context;
    private readonly IHashService _hash;
    private readonly MobileSecurityOptions _mobileSecurityOptions;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="context">The EntityFramework context</param>
    /// <param name="hash">Implementation of the IHashService to be used to generate the hashes. It can either be `HashService` or `ClearTextService`.</param>
    /// <param name="mobileSecurityOptions">Options for specifying the DeviceLock feature toggle. If MobileSecurity:LockDevice is enabled (set to `true` in settings), the first login of the observer will store the UniqueDeviceId and only that device will be allowed to login.</param>
    public ObserverAuthenticationQueryHandler(VoteMonitorContext context, IHashService hash, IOptions<MobileSecurityOptions> mobileSecurityOptions)
    {
        _context = context;
        _hash = hash;
        _mobileSecurityOptions = mobileSecurityOptions.Value;
    }

    public async Task<RegisteredObserverModel> Handle(ObserverApplicationUser message, CancellationToken cancellationToken)
    {
        var hashValue = _hash.GetHash(message.Pin);

        // Check for username and hash
        var userQuery = _context.Observers.Where(o => o.Pin == hashValue && o.Phone.Trim() == message.Phone.Trim());

        // Only if device lock is enabled verify the DeviceId
        if (_mobileSecurityOptions.LockDevice)
        {
            userQuery = userQuery.Where(observer => string.IsNullOrWhiteSpace(observer.MobileDeviceId)
                                                    || observer.MobileDeviceId == message.MobileDeviceId
                                                    || (observer.MobileDeviceIdType == MobileDeviceIdType.UserGeneratedGuid && message.MobileDeviceIdType == MobileDeviceIdType.FcmToken));
        }

        var userInfo = await userQuery.FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (userInfo == null)
        {
            return new RegisteredObserverModel
            {
                IsAuthenticated = false
            };
        }

        return new RegisteredObserverModel
        {
            ObserverId = userInfo.Id,
            IdNgo = userInfo.IdNgo,
            IsAuthenticated = true,
            ShouldRegisterMobileDeviceId = _mobileSecurityOptions.LockDevice &&
                                           (string.IsNullOrWhiteSpace(userInfo.MobileDeviceId)
                                            || (userInfo.MobileDeviceIdType == MobileDeviceIdType.UserGeneratedGuid && message.MobileDeviceIdType == MobileDeviceIdType.FcmToken))
        };
    }
}