using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIG.Lib.Tools.InkAtlas;

internal class Texture
{
    public DepotPath DepotPath { get; set; } = new();
    public string Flags { get; set; } = "Default";
}