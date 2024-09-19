using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIG.Lib.Tools.InkAtlas;

internal class Data
{
    public int Version { get; set; } = 195;
    public int BuildVersion { get; set; } = 0;
    public RootChunk RootChunk { get; set; } = new();
    public List<object> EmbeddedFiles { get; set; } = [];
}