using Newtonsoft.Json;

namespace WIG.Lib.Tools.InkAtlas;

internal class RootChunk
{
    [JsonProperty("$type")]
    public string Type { get; set; } = "inkTextureAtlas";

    [JsonProperty("activeTexture")]
    public string ActiveTexture { get; set; } = "StaticTexture";

    [JsonProperty("cookingPlatform")]
    public string CookingPlatform { get; set; } = "PLATFORM_None";

    [JsonProperty("dynamicTexture")]
    public DynamicTexture DynamicTexture { get; set; } = new();

    [JsonProperty("dynamicTextureSlot")]
    public DynamicTextureSlot DynamicTextureSlot { get; set; } = new();

    [JsonProperty("isSingleTextureMode")]
    public int IsSingleTextureMode { get; set; } = 1;

    [JsonProperty("parts")]
    public List<object> Parts { get; set; } = [];

    [JsonProperty("slices")]
    public List<object> Slices { get; set; } = [];

    [JsonProperty("slots")]
    public Slots Slots { get; set; } = new();

    [JsonProperty("texture")]
    public Texture Texture { get; set; } = new() { DepotPath = new DepotPath() { Storage = "string", Value = "" }, Flags = "Default" };

    [JsonProperty("textureResolution")]
    public string TextureResolution { get; set; } = "UltraHD_3840_2160";
}