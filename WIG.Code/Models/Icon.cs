using AetherUtils.Core.Logging;
using Newtonsoft.Json;
using System.ComponentModel;
using WolvenIconGenerator.Utility;

namespace WolvenIconGenerator.Models
{
    /// <summary>
    /// Represents an Icon that was created by the user or extracted from an imported <c>.archive</c> file.
    /// </summary>
    public class Icon : INotifyPropertyChanged, ICloneable, IEquatable<Icon>
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string? _imagePath = "\\path\\to\\custom\\image\\file";
        private string? _archivePath = "\\path\\to\\archive\\file";
        private string? _iconName = "custom_icon";
        private string? _sha256HashOfArchiveFile = string.Empty;

        private RadioExtCustomIcon? _customIcon;

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

        [JsonProperty("customIcon")]
        public RadioExtCustomIcon? CustomIcon
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
        public Icon() { }

        /// <summary>
        /// Create a new Icon object from a path to a <c>.png</c> image file.
        /// </summary>
        /// <param name="imagePath">The path to the image on disk.</param>
        public static Icon FromPath(string imagePath)
        {
            return new Icon(imagePath);
        }

        private Icon(string imagePath)
        {
            try
            {
                if (!Path.GetExtension(imagePath).Equals(".png"))
                    throw new ArgumentException("The image file must be a .png file.");

                ImagePath = imagePath;
                IconName = imagePath.Split('\\').Last();
                IconImage = Image.FromFile(imagePath);
            }
            catch (Exception e)
            {
                AuLogger.GetCurrentLogger<Icon>().Error(e);
            }
        }

        /// <summary>
        /// Create a new Icon object from a path to a <c>.png</c> image file and a path to an archive file.
        /// </summary>
        /// <param name="imagePath">The path to the <c>.png</c> file for this icon.</param>
        /// <param name="archivePath">The path to the <c>.archive</c> file for this icon.</param>
        public Icon(string imagePath, string archivePath)
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
        public Icon(string imagePath, string archivePath, string iconName) :
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

                IconImage ??= Image.FromFile(ImagePath);
                IconImage.Tag = ImagePath;

                return IconImage != null;
            }
            catch (Exception e)
            {
                AuLogger.GetCurrentLogger<Icon>("EnsureImage").Error(e);
            }
            return false;
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
            return new Icon()
            {
                ImagePath = _imagePath,
                IconName = _iconName,
                ArchivePath = _archivePath,
                Sha256HashOfArchiveFile = _sha256HashOfArchiveFile
            };
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Icon);
        }

        public bool Equals(Icon? other)
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
