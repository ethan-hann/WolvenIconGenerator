using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WIG.Lib.Tools.InkAtlas;

internal class DynamicTexture
{
    public DepotPath DepotPath { get; set; } = new();
    public string Flags { get; set; } = "Default";
}