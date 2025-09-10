using Newtonsoft.Json;
using System.ComponentModel;
using AetherUtils.Core.Files;
using AetherUtils.Core.Logging;
using WIG.Lib.Utility;

namespace WIG.Lib.Models.Audio
{
    public sealed class WolvenAudio : INotifyPropertyChanged, ICloneable, IEquatable<WolvenAudio>
    {
        private string? _archivePath = "\\path\\to\\audio\\archive";
        private string? _vanillaStationName = string.Empty;
        private string? _sha256HashOfArchiveFile = string.Empty;

        private List<ReplacementTrack> _files = [];

        private Guid? _audioId;

        /// <summary>
        /// The <see cref="VanillaStation"/> that this audio archive corresponds to.
        /// </summary>
        public VanillaStation? VanillaStation { get; private set; }

        /// <summary>
        /// Empty constructor for JSON deserialization.
        /// </summary>
        public WolvenAudio()
        {
        }

        /// <summary>
        /// Create a new WolvenAudio instance with the specified vanilla station name, archive path, and replacement files.
        /// </summary>
        /// <param name="vanillaStationName"></param>
        /// <param name="archivePath"></param>
        /// <param name="files">The <see cref="ReplacementTrack"/> files used to replace the station.</param>
        /// <exception cref="ArgumentException">If the archive path does not exist or the vanilla station does not exist.</exception>
        public WolvenAudio(string vanillaStationName, string archivePath, List<ReplacementTrack> files)
        {
            try
            {
                if (!Path.GetExtension(archivePath).Equals(".archive"))
                    throw new ArgumentException("The archive file must be a .archive file.");

                if (!FileHelper.DoesFileExist(archivePath))
                    throw new ArgumentException($"The archive file '{archivePath}' does not exist.");

                ArchivePath = archivePath;
                VanillaStation = AudioManager.Instance.GetVanillaStationByName(vanillaStationName);

                if (VanillaStation == null)
                    throw new ArgumentException($"The vanilla station '{vanillaStationName}' does not exist.");

                VanillaStationName = vanillaStationName;

                Sha256HashOfArchiveFile = HashUtils.ComputeSha256Hash(archivePath, true);

                var validStation = EnsureStationWemIds();
                if (!validStation)
                    throw new ArgumentException("One or more replacement tracks have invalid WEM IDs.");
            }
            catch (Exception ex)
            {
                AuLogger.GetCurrentLogger<WolvenAudio>().Error(ex, "Failed to create WolvenAudio instance.");
            }

            _files = files;
        }

        /// <summary>
        /// Checks that all replacement tracks have valid WEM IDs according to the associated <see cref="VanillaStation"/>.
        /// </summary>
        /// <returns></returns>
        public bool EnsureStationWemIds()
        {
            if (VanillaStation == null)
            {
                AuLogger.GetCurrentLogger<WolvenAudio>().Error("Cannot ensure WEM IDs because the VanillaStation is null.");
                return false;
            }

            var validWemIds = new HashSet<string>(VanillaStation.Tracks.SelectMany(t => t.WemIds));
            foreach (var file in Files.Where(file => !validWemIds.Contains(file.WemId)))
            {
                AuLogger.GetCurrentLogger<WolvenAudio>().Error($"Invalid WEM ID '{file.WemId}' for replacement track '{file.VanillaTrackName}' in station '{VanillaStationName}'.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// The unique id for this icon.
        /// </summary>
        [JsonProperty("audioId")]
        public Guid? AudioId
        {
            get => _audioId;
            set
            {
                _audioId = value;
                OnPropertyChanged(nameof(AudioId));
            }
        }

        /// <summary>
        /// The path to the archive file that the game uses to load the icon from.
        /// </summary>
        [JsonProperty("archivePath")]
        public string? ArchivePath
        {
            get => _archivePath;
            set
            {
                _archivePath = value;
                OnPropertyChanged(nameof(ArchivePath));
            }
        }

        /// <summary>
        /// The SHA256 hash of the archive file that the game uses to load the icon from.
        /// </summary>
        [JsonProperty("sha256HashOfArchiveFile")]
        public string? Sha256HashOfArchiveFile
        {
            get => _sha256HashOfArchiveFile;
            set
            {
                _sha256HashOfArchiveFile = value;
                OnPropertyChanged(nameof(Sha256HashOfArchiveFile));
            }
        }

        /// <summary>
        /// The name of the icon. Does not have to be unique.
        /// </summary>
        [JsonProperty("vanillaStationName")]
        public string? VanillaStationName
        {
            get => _vanillaStationName;
            set
            {
                _vanillaStationName = value;
                OnPropertyChanged(nameof(VanillaStationName));
            }
        }

        /// <summary>
        /// The list of <see cref="ReplacementTrack"/> files used to replace the station.
        /// </summary>
        [JsonProperty("files")]
        public List<ReplacementTrack> Files
        {
            get => _files;
            set
            {
                _files = value;
                OnPropertyChanged(nameof(Files));
            }
        }

        /// <summary>
        /// Event that is raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new WolvenAudio()
            {
                VanillaStationName = _vanillaStationName,
                ArchivePath = _archivePath,
                AudioId = _audioId,
                Sha256HashOfArchiveFile = _sha256HashOfArchiveFile
            };
        }

        /// <summary>
        /// Get a string representation of the vanilla audio station.
        /// </summary>
        /// <returns>The string representation of the station.</returns>
        public override string ToString()
        {
            return $"{VanillaStationName}";
        }

        /// <summary>
        /// <inheritdoc cref="object.Equals(object?)"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(object? other)
        {
            return Equals(other as WolvenAudio);
        }

        /// <summary>
        /// <inheritdoc cref="IEquatable{T}.Equals(T?)"/>
        /// </summary>
        /// <param name="other">The <see cref="WolvenAudio"/> to compare with.</param>
        /// <returns></returns>
        public bool Equals(WolvenAudio? other)
        {
            if (other == null) return false;
            return VanillaStationName == other.VanillaStationName &&
                   ArchivePath == other.ArchivePath &&
                   Sha256HashOfArchiveFile == other.Sha256HashOfArchiveFile;
        }

        /// <summary>
        /// <inheritdoc cref="object.GetHashCode"/>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Sha256HashOfArchiveFile, VanillaStationName, ArchivePath);
        }
    }
}
