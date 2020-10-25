using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Observer.Models
{
    public class NewObserverModel
    {
        [Required]
        public string Phone { get; set; }

        [Required]
        public string Pin { get; set; }

        [Required]
        public string Name { get; set; }
        public bool SendSMS { get; set; }
    }
}
