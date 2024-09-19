using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WIG.Lib.Tools.InkAtlas;

internal class Slots
{
    public List<Element> Elements { get; set; } =
    [
        new Element
        {
            Type = "inkTextureSlot",
            Parts = [],
            Slices = [],
            Texture = new Texture()
            {
                DepotPath = new DepotPath()
                {
                    Storage = "string"
                },
                Flags = "Soft"
            }
        },
        new Element
        {
            Type = "inkTextureSlot",
            Parts = [],
            Slices = [],
            Texture = new Texture()
            {
                DepotPath = new DepotPath()
                {
                    Storage = "string"
                },
                Flags = "Soft"
            }
        }
    ];
}