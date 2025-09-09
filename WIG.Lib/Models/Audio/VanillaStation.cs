using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIG.Lib.Models.Audio
{
    /// <summary>
    /// Represents a single vanilla radio station from the game.
    /// </summary>
    /// <param name="stationName">The name of the station as it appears in game.</param>
    /// <param name="tracks">A list of <see cref="AudioTrack"/> objects that define the music for the station.</param>
    public class VanillaStation(string stationName, List<AudioTrack> tracks)
    {
        /// <summary>
        /// The name of the station as it appears in game.
        /// </summary>
        public string StationName { get; set; } = stationName;

        /// <summary>
        /// A list of <see cref="AudioTrack"/> objects that define the music for the station.
        /// </summary>
        public List<AudioTrack> Tracks { get; set; } = tracks;
    }
}
