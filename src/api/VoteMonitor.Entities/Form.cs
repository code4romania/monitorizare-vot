using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMonitor.Entities
{
    public partial class Form
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, MaxLength(2), JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [Required]
        public string Description { get; set; }

        [Required, JsonProperty(PropertyName = "ver")]
        public int CurrentVersion { get; set; }
        public virtual ICollection<FormSection> FormSections { get; set; }
    }
}
