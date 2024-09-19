using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WIG.Lib.Tools.InkAtlas;

internal class DepotPath
{
    [JsonProperty("$type")]
    public string Type { get; set; } = "ResourcePath";
    [JsonProperty("$storage")]
    public string Storage { get; set; } = "uint64";
    [JsonProperty("$value")]
    public string Value { get; set; } = "0";
}