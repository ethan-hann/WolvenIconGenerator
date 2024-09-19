using Newtonsoft.Json;

namespace WIG.Lib.Tools.InkAtlas;

internal class PartName
{
    [JsonProperty("$type")]
    public string Type { get; set; } = string.Empty;
    [JsonProperty("$storage")]
    public string Storage { get; set; } = string.Empty;
    [JsonProperty("$value")]
    public string Value { get; set; } = string.Empty;
}