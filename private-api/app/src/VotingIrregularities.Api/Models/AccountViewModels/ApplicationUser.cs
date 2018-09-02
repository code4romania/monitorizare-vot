using System.ComponentModel.DataAnnotations;
using MediatR;

namespace VotingIrregularities.Api.Models.AccountViewModels
{
    /// <summary>
    /// Model received from client applications in order to perform the authentication
    /// </summary>
    public class ApplicationUser : IRequest<ModelObservatorInregistrat>
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

    public class ModelObservatorInregistrat
    {
        public bool EsteAutentificat { get; set; }

        public int IdObservator { get; set; }

        public bool PrimaAutentificare { get; set; }
    }
}
