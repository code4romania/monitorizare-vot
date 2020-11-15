using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.County.Models
{
    public class UpdateCountyRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        public int NumberOfPollingStations { get; set; }
        public bool Diaspora { get; set; }
        public int Order { get; set; }
    }
}