using VoteMonitor.Entities;

namespace VoteMonitor.Api.Auth.Models
{
    public class RegisteredObserverModel
    {
        public bool IsAuthenticated { get; set; }

        public int ObserverId { get; set; }
        /// <summary>
        /// The <see cref="Observer.MobileDeviceId"/> is registered if the lock device feature is on and the user is logging for the first time,
        /// or the user is logging for the first time with <seealso cref="MobileDeviceIdType.FcmToken" /> instead of <seealso cref="MobileDeviceIdType.UserGeneratedGuid" />.
        /// </summary>
        public bool ShouldRegisterMobileDeviceId { get; set; }
        public int IdNgo { get; set; }
        public MobileDeviceIdType MobileDeviceIdType { get; set; }
    }
}