using System.Text.Json.Serialization;

namespace VoteMonitor.Api.Form.Models;

public class FormDetailsModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("currentVersion")]
    public int CurrentVersion { get; set; }

    // quick and dirty fix to sync iOS and Android
    [JsonPropertyName("ver")]
    public int CurrentVersionOld { get; set; }

    [JsonPropertyName("diaspora")]
    public bool Diaspora { get; set; }

    [JsonPropertyName("order")]
    public int Order { get; set; }

    [JsonPropertyName("draft")]
    public bool Draft { get; set; }
}
