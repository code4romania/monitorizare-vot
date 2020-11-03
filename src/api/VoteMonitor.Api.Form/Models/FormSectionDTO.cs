using System.Text.Json.Serialization;
using System.Collections.Generic;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Models
{
    public class FormSectionDTO : IHierarchicalEntity<QuestionDTO>, IIdentifiableEntity
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
        public ICollection<QuestionDTO> Questions { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        ICollection<QuestionDTO> IHierarchicalEntity<QuestionDTO>.Children { get => Questions; set => Questions = value; }
    }
}
