using Newtonsoft.Json;

namespace WIG.Lib.Tools.InkAtlas;

internal class DynamicTextureSlot
{
    [JsonProperty("$type")]
    public string Type { get; set; } = "inkDynamicTextureSlot";

    [JsonProperty("parts")]
    public List<object> Parts { get; set; } = [];

    [JsonProperty("texture")]
    public Texture Texture { get; set; } = new();
}