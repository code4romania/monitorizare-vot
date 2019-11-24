using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Auth.Models
{
    public class AuthenticateUserRequest
    {
        [Required]
        public string User { get; set; }
        [Required]
        public string Password { get; set; }
        public string UniqueId { get; set; }
    }
}