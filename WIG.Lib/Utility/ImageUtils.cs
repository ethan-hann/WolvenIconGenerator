// ImageUtils.cs : WIG.Lib
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

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using AetherUtils.Core.Logging;

namespace WIG.Lib.Utility;

/// <summary>
/// Contains utility methods for image handling.
/// </summary>
public sealed class ImageUtils
{
    /// <summary>
    /// Load and optimize an image from the specified file.
    /// </summary>
    /// <param name="file">The file to load as an image.</param>
    /// <param name="maxWidth">Optional max width to down sample the image, 0 to disable.</param>
    /// <param name="maxHeight">Optional max height to down sample the image, 0 to disable.</param>
    /// <returns>The optimized <see cref="Image"/> or <c>null</c> if the image couldn't be loaded.</returns>
    public static Image? LoadAndOptimizeImage(string file, int maxWidth = 0, int maxHeight = 0)
    {
        try
        {
            // Ensure it's a PNG file
            if (!IsPngFile(file))
                return null;

            using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);

            // Load image from stream and create a copy to release file lock
            var originalImage = stream.Length == 0 ? null : Image.FromStream(stream);
            if (originalImage == null) return null;

            // Optional down sampling to fit within max dimensions
            if (maxWidth > 0 && maxHeight > 0) originalImage = DownsampleImage(originalImage, maxWidth, maxHeight);

            // Optimize the image for memory usage (convert to 24bpp RGB format)
            return OptimizeImageForMemory(originalImage);
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<ImageUtils>("LoadImage").Error(e.Message);
            return null;
        }
    }

    /// <summary>
    /// Downsample the image to fit within the specified width and height, preserving aspect ratio.
    /// </summary>
    /// <param name="original">The original image.</param>
    /// <param name="maxWidth">The maximum width to fit.</param>
    /// <param name="maxHeight">The maximum height to fit.</param>
    /// <returns>A downsampled image.</returns>
    public static Bitmap DownsampleImage(Image original, int maxWidth, int maxHeight)
    {
        // Calculate the scaling factor
        var ratioX = (float)maxWidth / original.Width;
        var ratioY = (float)maxHeight / original.Height;
        var ratio = Math.Min(ratioX, ratioY);

        // Calculate the new width and height based on the ratio
        var newWidth = (int)(original.Width * ratio);
        var newHeight = (int)(original.Height * ratio);

        var resizedImage = new Bitmap(newWidth, newHeight);
        using (var g = Graphics.FromImage(resizedImage))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(original, 0, 0, newWidth, newHeight);
        }

        return resizedImage;
    }

    /// <summary>
    /// Optimize the image for memory usage by converting it to a format that preserves transparency if needed.
    /// </summary>
    /// <param name="original">The original image.</param>
    /// <returns>An optimized image that preserves transparency for PNGs.</returns>
    private static Bitmap OptimizeImageForMemory(Image original)
    {
        // Check if the original image has an alpha channel (supports transparency)
        var pixelFormat = original.PixelFormat == PixelFormat.Format32bppArgb
                          || original.PixelFormat == PixelFormat.Format32bppPArgb
                          || original.PixelFormat == PixelFormat.Format32bppRgb
            ? PixelFormat.Format32bppArgb
            : PixelFormat.Format24bppRgb; // Fallback to 24bpp if no transparency

        var optimizedBitmap = new Bitmap(original.Width, original.Height, pixelFormat);
        using (var g = Graphics.FromImage(optimizedBitmap))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height));
        }

        return optimizedBitmap;
    }

    /// <summary>
    /// Determines if the specified file is a PNG file.
    /// </summary>
    /// <param name="file">The file to check.</param>
    /// <returns><c>true</c> if the file is a .png; <c>false</c> otherwise.</returns>
    public static bool IsPngFile(string file)
    {
        return Path.GetExtension(file).Equals(".png", StringComparison.OrdinalIgnoreCase);
    }
}