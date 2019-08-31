namespace VoteMonitor.Api.Observer.Models
{
    public class NewObserverModel
    {
        public int IdNgo { get; set; }
        public string Phone { get; set; }
        public string Pin { get; set; }
        public string Name { get; set; }
        public bool SendSMS { get; set; }
    }
}
