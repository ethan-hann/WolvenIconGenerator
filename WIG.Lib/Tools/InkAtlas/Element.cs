using Newtonsoft.Json;

namespace WIG.Lib.Tools.InkAtlas;

internal class Element
{
    [JsonProperty("$type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("parts")]
    public List<InkTextureAtlasMapper> Parts { get; set; } = [];

    [JsonProperty("slices")]
    public List<object> Slices { get; set; } = [];

    [JsonProperty("texture")]
    public Texture Texture { get; set; } = new();
}