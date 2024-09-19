using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Image = SixLabors.ImageSharp.Image;

namespace WIG.Lib.Tools.InkAtlas
{
    internal class ImageData
    {
        public string Name { get; set; } = string.Empty;
        public required Image Image { get; set; }
    }
}
