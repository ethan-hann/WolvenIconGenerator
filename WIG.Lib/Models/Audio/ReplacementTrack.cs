
namespace WIG.Lib.Models.Audio
{
    /// <summary>
    /// Holds information about a track that is replacing a vanilla track in a WolvenAudio archive.
    /// The properties are immutable after creation to preserve the created archive's state.
    /// </summary>
    public sealed class ReplacementTrack
    {
        /// <summary>
        /// The name of the vanilla track being replaced.
        /// </summary>
        public string VanillaTrackName { get; }

        /// <summary>
        /// The WEM ID of the vanilla track being replaced.
        /// </summary>
        public string WemId { get; }

        /// <summary>
        /// The file path to the replacement audio file.
        /// </summary>
        public string ReplacementFilePath { get; }

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
        /// Gets a string representation of the ReplacementTrack.
        /// Format: "VanillaTrackName (WemId) -> ReplacementFilePath"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{VanillaTrackName} ({WemId}) -> {ReplacementFilePath}";
        }
    }
}
