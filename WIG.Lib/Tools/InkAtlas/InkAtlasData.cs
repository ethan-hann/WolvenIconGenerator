using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIG.Lib.Tools.InkAtlas;

internal class InkAtlasData
{
    public Header Header { get; set; } = new();
    public Data Data { get; set; } = new();
}