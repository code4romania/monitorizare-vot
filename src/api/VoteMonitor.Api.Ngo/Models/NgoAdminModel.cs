namespace VoteMonitor.Api.Ngo.Models
{
    public class NgoAdminModel
    {
        public int Id { get; set; }
        public int IdNgo { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
    }
}