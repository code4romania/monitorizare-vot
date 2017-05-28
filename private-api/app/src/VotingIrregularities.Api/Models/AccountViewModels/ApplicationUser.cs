using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace VotingIrregularities.Api.Models.AccountViewModels
{
    public class ApplicationUser : IRequest<ModelObservatorInregistrat>
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

    public class ModelObservatorInregistrat
    {
        public bool EsteAutentificat { get; set; }

        public int IdObservator { get; set; }

        public bool PrimaAutentificare { get; set; }
    }
}
