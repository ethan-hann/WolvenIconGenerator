using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIG.Lib.Models.Audio
{
    /// <summary>
    /// Represents an audio track with its name, artist, and associated WEM IDs.
    /// </summary>
    /// <param name="trackName">The track name.</param>
    /// <param name="trackArtist">The track's artist.</param>
    /// <param name="trackDurations">The track's duration, in seconds, as a list.</param>
    /// <param name="wemIds">A set of WEM filename ids, as strings.</param>
    public class AudioTrack(string trackName, string trackArtist, List<float> trackDurations, HashSet<string> wemIds)
    {
        public readonly string TrackName = trackName;
        public readonly string TrackArtist = trackArtist;
        public readonly List<float> TrackDuration = trackDurations;
        public readonly HashSet<string> WemIds = wemIds;
    }
}
