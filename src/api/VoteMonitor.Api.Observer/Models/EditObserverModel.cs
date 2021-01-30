using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Observer.Models
{
    public class EditObserverModel
    {
        [Required]
        public int? ObserverId { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Pin { get; set; }
    }
}
