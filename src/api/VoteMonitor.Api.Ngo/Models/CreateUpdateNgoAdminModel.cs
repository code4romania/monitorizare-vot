using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Ngo.Models
{
    public class CreateUpdateNgoAdminModel
    {
        [Required] [StringLength(50)] public string Account { get; set; }
        [Required] [StringLength(100)] public string Password { get; set; }
    }
}