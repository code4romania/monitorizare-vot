namespace VoteMonitor.Api.Auth.Models
{
    public class AuthenticationResponseModel
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}