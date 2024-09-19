using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WIG.Lib.Tools.InkAtlas;

internal class ClippingRectInUvCoords
{
    [JsonProperty("$type")]
    public string Type { get; set; } = string.Empty;
    public double Bottom { get; set; }
    public double Left { get; set; }
    public double Right { get; set; }
    public double Top { get; set; }
}