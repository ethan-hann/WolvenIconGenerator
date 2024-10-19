// ClippingRectInUVCoords.cs : WIG.Lib
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

internal class ClippingRectInUvCoords
{
    [JsonProperty("$type")] public string Type { get; set; } = string.Empty;

    public double Bottom { get; set; }
    public double Left { get; set; }
    public double Right { get; set; }
    public double Top { get; set; }
}