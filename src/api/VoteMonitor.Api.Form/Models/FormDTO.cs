using System.Text.Json.Serialization;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Models;

public class FormDTO : IHierarchicalEntity<FormSectionDTO>
{
    public int Id { get; set; }
    public string Code { get; set; }
    public int CurrentVersion { get; set; }
    public string Description { get; set; }
    public ICollection<FormSectionDTO> FormSections { get; set; }
    public bool Diaspora { get; set; }
    public bool Draft { get; set; }
    public int Order { get; set; }

    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public ICollection<FormSectionDTO> Children { get => FormSections; set => FormSections = value; }
}
