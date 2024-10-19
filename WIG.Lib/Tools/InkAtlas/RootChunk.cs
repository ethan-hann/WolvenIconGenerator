// RootChunk.cs : WIG.Lib
// Copyright (C) 2024  Ethan Hann
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using Newtonsoft.Json;

namespace WIG.Lib.Tools.InkAtlas;

internal class RootChunk
{
    [JsonProperty("$type")] public string Type { get; set; } = "inkTextureAtlas";

    [JsonProperty("activeTexture")] public string ActiveTexture { get; set; } = "StaticTexture";

    [JsonProperty("cookingPlatform")] public string CookingPlatform { get; set; } = "PLATFORM_None";

    [JsonProperty("dynamicTexture")] public DynamicTexture DynamicTexture { get; set; } = new();

    [JsonProperty("dynamicTextureSlot")] public DynamicTextureSlot DynamicTextureSlot { get; set; } = new();

    [JsonProperty("isSingleTextureMode")] public int IsSingleTextureMode { get; set; } = 1;

    [JsonProperty("parts")] public List<object> Parts { get; set; } = [];

    [JsonProperty("slices")] public List<object> Slices { get; set; } = [];

    [JsonProperty("slots")] public Slots Slots { get; set; } = new();

    [JsonProperty("texture")]
    public Texture Texture { get; set; } = new()
        { DepotPath = new DepotPath { Storage = "string", Value = "" }, Flags = "Default" };

    [JsonProperty("textureResolution")] public string TextureResolution { get; set; } = "UltraHD_3840_2160";
}