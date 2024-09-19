using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIG.Lib.Tools.InkAtlas;

internal class InkTextureAtlasMapper
{
    [JsonProperty("$type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonProperty("clippingRectInPixels")]
    public ClippingRectInPixels ClippingRectInPixels { get; set; } = new();

    [JsonProperty("clippingRectInUVCoords")]
    public ClippingRectInUvCoords ClippingRectInUvCoords { get; set; } = new();

    [JsonProperty("partName")]
    public PartName PartName { get; set; } = new();
}