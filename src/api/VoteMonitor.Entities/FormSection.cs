using System.Collections.Generic;

namespace VoteMonitor.Entities
{
    public partial class FormSection
    {
        public FormSection()
        {
            Questions = new HashSet<Question>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}
