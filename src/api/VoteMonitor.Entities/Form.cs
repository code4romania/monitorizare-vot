using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMonitor.Entities
{
    public partial class Form : IHierarchicalEntity<FormSection>, IOrderedEntity, IIdentifiableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Code { get; set; }

        [Required]
        public string Description { get; set; }

        public int CurrentVersion { get; set; }

        public bool Diaspora { get; set; }

        public bool Draft { get; set; }

		public int Order { get; set; }

		public virtual ICollection<FormSection> FormSections { get; set; }

        int IOrderedEntity.OrderNumber { get => Order; set => Order = value; }

        ICollection<FormSection> IHierarchicalEntity<FormSection>.Children { get => FormSections; set => FormSections = value; }
    }
}
