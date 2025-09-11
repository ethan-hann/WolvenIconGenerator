// VanillaStation.cs : WIG.Lib
// Copyright (C) 2025  Ethan Hann
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

namespace WIG.Lib.Models.Audio;

/// <summary>
/// Represents a single vanilla radio station from the game.
/// </summary>
/// <param name="stationName">The name of the station as it appears in game.</param>
/// <param name="tracks">A list of <see cref="AudioTrack"/> objects that define the music for the station.</param>
public class VanillaStation(string stationName, List<AudioTrack> tracks)
{
    /// <summary>
    /// Empty constructor for JSON deserialization.
    /// </summary>
    public VanillaStation() : this(string.Empty, [])
    {
    }

    /// <summary>
    /// The name of the station as it appears in game.
    /// </summary>
    [JsonProperty("stationName")]
    public string StationName { get; set; } = stationName;

    /// <summary>
    /// A list of <see cref="AudioTrack"/> objects that define the music for the station.
    /// </summary>
    [JsonProperty("tracks")]
    public List<AudioTrack> Tracks { get; set; } = tracks;
}