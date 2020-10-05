namespace VoteMonitor.Api.Auth.Models
{
    public class RegisteredObserverModel
    {
        public bool IsAuthenticated { get; set; }

        public int ObserverId { get; set; }

        public bool FirstAuthentication { get; set; }
        public int IdNgo { get; set; }
    }
}