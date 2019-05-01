using System.ComponentModel.DataAnnotations;
using MediatR;

namespace VotingIrregularities.Api.Models.AccountViewModels
{
    /// <summary>
    /// Model received from client applications in order to perform the authentication
    /// </summary>
    public class ApplicationUser : IRequest<RegisteredObserverModel>
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
        public string UDID { get; set; }
    }

    public class RegisteredObserverModel
    {
        public bool IsAuthenticated { get; set; }

        public int ObserverId { get; set; }

        public bool FirstAuthentication { get; set; }
    }
}
