using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace VotingIrregularities.Domain.Models
{
    public partial class FormVersion
    {
        [Key, Required, MaxLength(2), JsonProperty(PropertyName = "code")]
        public string Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required, JsonProperty(PropertyName = "ver")]
        public int CurrentVersion { get; set; }
    }
}
