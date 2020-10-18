using MediatR;
using System.ComponentModel.DataAnnotations;
using VoteMonitor.Api.Auth.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Auth.Queries
{
    /// <summary>
    /// Model received from client applications in order to perform the authentication
    /// </summary>
    public class ObserverApplicationUser : IRequest<RegisteredObserverModel>
    {
        /// <summary>
        /// User's phone number
        /// </summary>
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        /// <summary>
        /// PIN number used for authentication (should have received this number by SMS)
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Pin { get; set; }

        /// <summary>
        /// This is the unique identifier of the mobile device
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string MobileDeviceId { get; set; }


        /// <summary>
        /// This is the type of unique identifier from the mobile device.
        /// </summary>
        public MobileDeviceIdType MobileDeviceIdType { get; set; }
    }
}
