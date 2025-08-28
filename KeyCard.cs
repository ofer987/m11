using System.Text.Json.Serialization;

namespace M11;

public class KeyCard {
    [JsonPropertyName("view")]
    public string View { get; set; } = string.Empty;
}
