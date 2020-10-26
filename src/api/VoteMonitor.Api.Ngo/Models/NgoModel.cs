namespace VoteMonitor.Api.Ngo.Models
{
    public class NgoModel
    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public bool Organizer { get; set; }
        public bool IsActive { get; set; }
    }
}