using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Auth.Models
{
    public class AuthenticateUserRequestV2
    {
        [Required]
        public string User { get; set; }
        [Required]
        public string Password { get; set; }
        public string ChannelName { get; set; }
        public string FcmToken { get; set; }
    }
}
