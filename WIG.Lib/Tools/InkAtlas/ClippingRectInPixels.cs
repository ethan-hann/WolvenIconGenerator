using Newtonsoft.Json;

namespace WIG.Lib.Tools.InkAtlas;

internal class ClippingRectInPixels
{
    [JsonProperty("$type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("bottom")]
    public int Bottom { get; set; }

    [JsonProperty("left")]
    public int Left { get; set; }

    [JsonProperty("right")]
    public int Right { get; set; }

    [JsonProperty("top")]
    public int Top { get; set; }
}