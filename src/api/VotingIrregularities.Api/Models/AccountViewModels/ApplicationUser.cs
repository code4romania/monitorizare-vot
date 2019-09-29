using System.ComponentModel.DataAnnotations;
using MediatR;
using MonitorizareVot.Ong.Api.Models;

namespace VotingIrregularities.Api.Models.AccountViewModels
{
    public class ApplicationUser : IRequest<UserInfo>
    {
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}
