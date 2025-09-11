// AudioTrack.cs : WIG.Lib
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

namespace WIG.Lib.Models.Audio;

/// <summary>
/// Represents an audio track with its name, artist, and associated WEM IDs.
/// </summary>
/// <param name="trackName">The track name.</param>
/// <param name="trackArtist">The track's artist.</param>
/// <param name="trackDurations">The track's duration, in seconds, as a list.</param>
/// <param name="wemIds">A set of WEM filename ids, as strings.</param>
public class AudioTrack(string trackName, string trackArtist, List<float> trackDurations, HashSet<string> wemIds)
{
    public readonly string TrackArtist = trackArtist;
    public readonly List<float> TrackDuration = trackDurations;
    public readonly string TrackName = trackName;
    public readonly HashSet<string> WemIds = wemIds;
}