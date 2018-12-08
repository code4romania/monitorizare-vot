using System.ComponentModel.DataAnnotations;
using MediatR;

namespace VotingIrregularities.Api.Models.AccountViewModels
{
    public class ApplicationUser : IRequest<RegisteredObserverModel>
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Pin { get; set; }

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
