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
        }
    ];
}