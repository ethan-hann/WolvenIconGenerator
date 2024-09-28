using AetherUtils.Core.Logging;
using Newtonsoft.Json;
using System.ComponentModel;
using WIG.Lib.Utility;

namespace WIG.Lib.Models
{
    /// <summary>
    /// Represents an Icon that was created by the user or extracted from an imported <c>.archive</c> file.
    /// </summary>
    public class WolvenIcon : INotifyPropertyChanged, ICloneable, IEquatable<WolvenIcon>
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string? _atlasName = "custom_icon_atlas";
        private string? _imagePath = "\\path\\to\\custom\\image\\file";
        private string? _archivePath = "\\path\\to\\archive\\file";
        private string? _originalArchivePath = "\\path\\to\\archive\\file";
        private string? _iconName = "custom_icon";
        private string? _sha256HashOfArchiveFile = string.Empty;

        private RadioExtCustomIcon _customIcon = new();

        private bool _isActive;

        /// <summary>
        /// The path to the icon.
        /// </summary>
        [JsonProperty("iconPath")]
        public string? ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
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
        /// The path to the original archive file before it was copied to the <see cref="ArchivePath"/>.
        /// </summary>
        [JsonProperty("originalArchivePath")]
        public string? OriginalArchivePath
        {
            get => _originalArchivePath;
            set
            {
                _originalArchivePath = value;
                OnPropertyChanged(nameof(OriginalArchivePath));
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
        /// The name of the atlas that the icon is stored in.
        /// </summary>
        [JsonProperty("atlasName")]
        public string? AtlasName
        {
            get => _atlasName;
            set
            {
                _atlasName = value;
                OnPropertyChanged(nameof(AtlasName));
            }
        }

        /// <summary>
        /// The name of the icon. Does not have to be unique.
        /// </summary>
        [JsonProperty("iconName")]
        public string? IconName
        {
            get => _iconName;
            set
            {
                _iconName = value;
                OnPropertyChanged(nameof(IconName));
            }
        }

        /// <summary>
        /// Get or set the active state of the icon for the associated station.
        /// </summary>
        [JsonProperty("isActive")]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }

        /// <summary>
        /// The custom icon object needed by RadioExt to reference the icon in game.
        /// </summary>
        [JsonProperty("customIcon")]
        public RadioExtCustomIcon CustomIcon
        {
            get => _customIcon;
            set
            {
                _customIcon = value;
                OnPropertyChanged(nameof(CustomIcon));
            }
        }

        /// <summary>
        /// The image object that represents the icon; <c>null</c> if the icon is not loaded from disk.
        /// </summary>
        public Image? IconImage { get; private set; }

        /// <summary>
        /// Empty constructor for JSON deserialization.
        /// </summary>
        public WolvenIcon() { }

        /// <summary>
        /// Create a new Icon object from a path to a <c>.png</c> image file.
        /// </summary>
        /// <param name="imagePath">The path to the image on disk.</param>
        public static WolvenIcon FromPath(string imagePath)
        {
            return new WolvenIcon(imagePath);
        }

        public static WolvenIcon FromArchive(string archivePath)
        {
            return new WolvenIcon(string.Empty, archivePath);
        }

        internal WolvenIcon(string imagePath)
        {
            try
            {
                if (!Path.GetExtension(imagePath).Equals(".png"))
                    throw new ArgumentException("The image file must be a .png file.");

                ImagePath = imagePath;
                IconName = Path.GetFileNameWithoutExtension(imagePath);
                IconImage = ImageUtils.LoadAndOptimizeImage(imagePath);
            }
            catch (Exception e)
            {
                AuLogger.GetCurrentLogger<WolvenIcon>().Error(e);
            }
        }

        /// <summary>
        /// Create a new Icon object from a path to a <c>.png</c> image file and a path to an archive file.
        /// </summary>
        /// <param name="imagePath">The path to the <c>.png</c> file for this icon.</param>
        /// <param name="archivePath">The path to the <c>.archive</c> file for this icon.</param>
        public WolvenIcon(string imagePath, string archivePath)
        {
            ImagePath = imagePath;
            ArchivePath = archivePath;
            Sha256HashOfArchiveFile = HashUtils.ComputeSha256Hash(archivePath, true);
        }

        /// <summary>
        /// Create a new Icon object from a path to a <c>.png</c> image file and a path to an archive file with a specified icon name.
        /// </summary>
        /// <param name="imagePath">The path to the <c>.png</c> file for this icon.</param>
        /// <param name="archivePath">The path to the <c>.archive</c> file for this icon.</param>
        /// <param name="iconName">The name of the icon. Does not need to be unique.</param>
        public WolvenIcon(string imagePath, string archivePath, string iconName) :
            this(imagePath, archivePath)
        {
            IconName = iconName;
        }

        /// <summary>
        /// Ensure the <see cref="IconImage"/> is loaded and ready to be used.
        /// </summary>
        public bool EnsureImage()
        {
            try
            {
                if (ImagePath == null)
                    throw new InvalidOperationException("The image path is null.");

                if (IconImage is { Tag: string path } && path.Equals(ImagePath))
                    return true;

                IconImage ??= ImageUtils.LoadAndOptimizeImage(ImagePath);

                if (IconImage == null)
                    throw new InvalidOperationException("The image could not be loaded.");

                IconImage.Tag = ImagePath;

                return IconImage != null;
            }
            catch (Exception e)
            {
                AuLogger.GetCurrentLogger<WolvenIcon>("EnsureImage").Error(e);
            }
            return false;
        }

        /// <summary>
        /// Performs a check on the icon's properties to ensure they are valid (not null and/or empty).
        /// </summary>
        /// <returns><c>true</c> if the icon is valid; <c>false</c> otherwise.</returns>
        public bool CheckIconValid()
        {
            var isValid = !string.IsNullOrEmpty(ImagePath) && !string.IsNullOrEmpty(ArchivePath) && !string.IsNullOrEmpty(Sha256HashOfArchiveFile);
            isValid &= Path.Exists(ArchivePath);
            isValid &= Path.Exists(ImagePath);
            return isValid;
        }

        /// <summary>
        /// Get a string representation of the icon in the format: <c>{IconName}: {IconId}</c>.
        /// </summary>
        /// <returns>The string representation of the icon.</returns>
        public override string ToString()
        {
            return $"{IconName}";
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Clone()
        {
            return new WolvenIcon()
            {
                ImagePath = _imagePath,
                IconName = _iconName,
                ArchivePath = _archivePath,
                Sha256HashOfArchiveFile = _sha256HashOfArchiveFile
            };
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as WolvenIcon);
        }

        public bool Equals(WolvenIcon? other)
        {
            if (other == null) return false;
            return ImagePath == other.ImagePath &&
                   ArchivePath == other.ArchivePath &&
                   Sha256HashOfArchiveFile == other.Sha256HashOfArchiveFile;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Sha256HashOfArchiveFile, ImagePath, ArchivePath);
        }
    }
}
