// WolvenIcon.cs : WIG.Lib
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
using AetherUtils.Core.Logging;
using Newtonsoft.Json;
using WIG.Lib.Utility;

namespace WIG.Lib.Models;

/// <summary>
/// Represents an Icon that was created by the user or extracted from an imported <c>.archive</c> file.
/// </summary>
public class WolvenIcon : INotifyPropertyChanged, ICloneable, IEquatable<WolvenIcon>
{
    private string? _archivePath = "\\path\\to\\archive\\file";

    private string? _atlasName = "custom_icon_atlas";

    private RadioExtCustomIcon _customIcon = new();
    private Guid? _iconId;
    private string? _iconName = "custom_icon";
    private string? _imagePath = "\\path\\to\\custom\\image\\file";

    private bool _isActive;
    private bool? _isFromArchive = false;
    private string? _originalArchivePath = "\\path\\to\\archive\\file";
    private string? _sha256HashOfArchiveFile = string.Empty;

    /// <summary>
    /// Empty constructor for JSON deserialization.
    /// </summary>
    public WolvenIcon()
    {
    }

    internal WolvenIcon(string imagePath)
    {
        try
        {
            if (!Path.GetExtension(imagePath).Equals(".png"))
                throw new ArgumentException("The image file must be a .png file.");

            ImagePath = imagePath;
            if (!Path.Exists(imagePath)) return;

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
    public WolvenIcon(string imagePath, string archivePath) : this(imagePath)
    {
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
    /// The unique id for this icon.
    /// </summary>
    [JsonProperty("iconId")]
    public Guid? IconId
    {
        get => _iconId;
        set
        {
            _iconId = value;
            OnPropertyChanged(nameof(IconId));
        }
    }

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
    /// Indicates if the icon was created from an <c>.archive</c> file or not.
    /// </summary>
    [JsonProperty("fromArchive")]
    public bool IsFromArchive
    {
        get => _isFromArchive ?? false;
        set
        {
            _isFromArchive = value;
            OnPropertyChanged(nameof(IsFromArchive));
        }
    }

    /// <summary>
    /// The image object that represents the icon; <c>null</c> if the icon is not loaded from disk.
    /// </summary>
    public Image? IconImage { get; private set; }

    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    public object Clone()
    {
        return new WolvenIcon
        {
            ImagePath = _imagePath,
            IconName = _iconName,
            ArchivePath = _archivePath,
            AtlasName = _atlasName,
            CustomIcon = _customIcon,
            IsActive = _isActive,
            IsFromArchive = _isFromArchive ?? false,
            IconId = _iconId,
            OriginalArchivePath = _originalArchivePath,
            Sha256HashOfArchiveFile = _sha256HashOfArchiveFile
        };
    }

    /// <summary>
    /// <inheritdoc cref="IEquatable{T}.Equals(T?)"/>
    /// </summary>
    /// <param name="other">The <see cref="WolvenIcon"/> to compare with.</param>
    /// <returns></returns>
    public bool Equals(WolvenIcon? other)
    {
        if (other == null) return false;
        return ImagePath == other.ImagePath &&
               ArchivePath == other.ArchivePath &&
               Sha256HashOfArchiveFile == other.Sha256HashOfArchiveFile;
    }

    /// <summary>
    /// Occurs whenever a property is changed.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Create a new Icon object from a path to a <c>.png</c> image file.
    /// </summary>
    /// <param name="imagePath">The path to the image on disk.</param>
    /// <returns>A new <see cref="WolvenIcon"/> instance.</returns>
    public static WolvenIcon FromPath(string imagePath)
    {
        return new WolvenIcon(imagePath);
    }

    /// <summary>
    /// Create a new Icon object from an <c>.archive</c> file.
    /// </summary>
    /// <param name="archivePath">The path to the archive file to create the icon from.</param>
    /// <returns>A new <see cref="WolvenIcon"/> instance.</returns>
    public static WolvenIcon FromArchive(string archivePath)
    {
        return new WolvenIcon(string.Empty, archivePath);
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

            //if (IconImage == null)
            //    throw new InvalidOperationException("The image could not be loaded.");

            if (IconImage != null)
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
    /// If the icon is from an archive, the <see cref="ArchivePath"/> and <see cref="Sha256HashOfArchiveFile"/> must be set but the <see cref="ImagePath"/> can be empty.
    /// </summary>
    /// <returns><c>true</c> if the icon is valid; <c>false</c> otherwise.</returns>
    public bool CheckIconValid()
    {
        var isValid = false;

        if (IsFromArchive)
        {
            isValid = !string.IsNullOrEmpty(ArchivePath) && !string.IsNullOrEmpty(Sha256HashOfArchiveFile);
            isValid &= Path.Exists(ArchivePath);
        }
        else
        {
            isValid = !string.IsNullOrEmpty(ImagePath) && !string.IsNullOrEmpty(ArchivePath) &&
                      !string.IsNullOrEmpty(Sha256HashOfArchiveFile);
            isValid &= Path.Exists(ArchivePath);
            isValid &= Path.Exists(ImagePath);
        }
        
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

    /// <summary>
    /// <inheritdoc cref="object.Equals(object?)"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as WolvenIcon);
    }

    /// <summary>
    /// <inheritdoc cref="object.GetHashCode"/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Sha256HashOfArchiveFile, ImagePath, ArchivePath);
    }
}