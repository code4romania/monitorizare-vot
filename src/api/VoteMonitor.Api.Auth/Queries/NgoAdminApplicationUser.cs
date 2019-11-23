using System.ComponentModel.DataAnnotations;
using MediatR;
using VoteMonitor.Api.Auth.Models;
using VoteMonitor.Api.Core.Models;

namespace VoteMonitor.Api.Auth.Queries
{
    public class NgoAdminApplicationUser : IRequest<UserInfo>
    {
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        public UserType UserType{ get; set; }
    }
}
