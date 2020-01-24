
using Newtonsoft.Json;
using System.Collections.Generic;

namespace VoteMonitor.Api.Form.Models
{
    public class FormVersionsModel
    {
        public List<FormDetailsModel> FormVersions { get; set; }
    }

    public class FormDetailsModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
     
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "ver")]
        public int CurrentVersion { get; set; }

        [JsonProperty(PropertyName = "diaspora")]
        public bool Diaspora { get; set; }

        [JsonProperty(PropertyName = "order")]
        public int Order { get; set; }
    }
}
