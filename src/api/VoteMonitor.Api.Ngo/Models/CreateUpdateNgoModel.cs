using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Ngo.Models
{
    public class CreateUpdateNgoModel
    {
        [Required] [StringLength(10)] public string ShortName { get; set; }
        [Required] [StringLength(200)] public string Name { get; set; }
        [Required] public bool Organizer { get; set; }
        [Required] public bool IsActive { get; set; }
    }
}