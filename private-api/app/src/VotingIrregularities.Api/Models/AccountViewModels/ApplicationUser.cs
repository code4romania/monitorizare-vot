using System.ComponentModel.DataAnnotations;

namespace VotingIrregularities.Api.Models.AccountViewModels
{
    public class ApplicationUser
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Pin { get; set; }

        [Required]
        public string UDID { get; set; }
    }
}
