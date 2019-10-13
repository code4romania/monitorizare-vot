namespace VoteMonitor.Api.Notification.Controllers
{
    [Route("api/v1/notification")]
    public class NotificationController : Controller
    {
        [HttpPost]
        [Route("register")]
        public void registerToken()
        { }
    }
}