// RadioExtCustomIcon.cs : WIG.Lib
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

using System.ComponentModel;
using Newtonsoft.Json;

namespace WIG.Lib.Models;

public class RadioExtCustomIcon : INotifyPropertyChanged, ICloneable, IEquatable<RadioExtCustomIcon>
{
    private string _inkAtlasPart = "custom_texture_part";
    private string _inkAtlasPath = "path\\to\\custom\\atlas.inkatlas";

    /// <summary>
    ///     Points to the <c>.inkatlas</c> that holds the icon texture.
    /// </summary>
    [JsonProperty("inkAtlasPath")]
    public string InkAtlasPath
    {
        get => _inkAtlasPath;
        set
        {
            _inkAtlasPath = value;
            OnPropertyChanged(nameof(InkAtlasPath));
        }
    }

    /// <summary>
    ///     Specifies which part of the <c>.inkatlas</c> should be used for the icon.
    /// </summary>
    [JsonProperty("inkAtlasPart")]
    public string InkAtlasPart
    {
        get => _inkAtlasPart;
        set
        {
            _inkAtlasPart = value;
            OnPropertyChanged(nameof(InkAtlasPart));
        }
    }

    public object Clone()
    {
        return new RadioExtCustomIcon
        {
            InkAtlasPath = InkAtlasPath,
            InkAtlasPart = InkAtlasPart
        };
    }

    public bool Equals(RadioExtCustomIcon? other)
    {
        if (other == null) return false;
        return InkAtlasPath == other.InkAtlasPath &&
               InkAtlasPart == other.InkAtlasPart;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as RadioExtCustomIcon);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(InkAtlasPath, InkAtlasPart);
    }
}