using System.Collections.Generic;

namespace VoteMonitor.Api.Form.Models
{
    public class FormSectionDTO
    {
        public FormSectionDTO()
        {
            Questions = new List<QuestionDTO>();
        }
        public string UniqueId { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int OrderNumber { get; set; }

        public List<QuestionDTO> Questions { get; set; }
    }
}
