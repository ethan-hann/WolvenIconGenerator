// ReplacementTrack.cs : WIG.Lib
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
/// Holds information about a track that is replacing a vanilla track in a WolvenAudio archive.
/// The properties are immutable after creation to preserve the created archive's state.
/// </summary>
public sealed class ReplacementTrack
{
    /// <summary>
    /// Creates a new instance of the <see cref="ReplacementTrack"/> class.
    /// </summary>
    /// <param name="vanillaTrackName"></param>
    /// <param name="wemId"></param>
    /// <param name="replacementFilePath"></param>
    public ReplacementTrack(string vanillaTrackName, string wemId, string replacementFilePath)
    {
        VanillaTrackName = vanillaTrackName;
        WemId = wemId;
        ReplacementFilePath = replacementFilePath;
    }

    /// <summary>
    /// Empty constructor for JSON deserialization.
    /// </summary>
    public ReplacementTrack()
    {
    }

    /// <summary>
    /// The name of the vanilla track being replaced.
    /// </summary>
    [JsonProperty("vanillaTrackName")]
    public string VanillaTrackName { get; private set; } = string.Empty;

    /// <summary>
    /// The WEM ID of the vanilla track being replaced.
    /// </summary>
    [JsonProperty("wemId")]
    public string WemId { get; private set; } = string.Empty;

    /// <summary>
    /// The file path to the replacement audio file.
    /// </summary>
    [JsonProperty("replacementFilePath")]
    public string ReplacementFilePath { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a string representation of the ReplacementTrack.
    /// Format: "VanillaTrackName (WemId) -> ReplacementFilePath"
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{VanillaTrackName} ({WemId}) -> {ReplacementFilePath}";
    }
}