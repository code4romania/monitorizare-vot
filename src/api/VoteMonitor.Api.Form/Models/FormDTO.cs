using System.Collections.Generic;

namespace VoteMonitor.Api.Form.Models
{
    public class FormDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int CurrentVersion { get; set; }
        public string Description { get; set; }
        public List<FormSectionDTO> FormSections { get; set; }
        public bool Diaspora { get; set; }
        public bool Draft { get; set; }
        public int Order { get; set; }
    }

}
