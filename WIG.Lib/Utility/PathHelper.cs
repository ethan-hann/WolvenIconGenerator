// PathHelper.cs : WIG.Lib
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

using AetherUtils.Core.Logging;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace WIG.Lib.Utility;

/// <summary>
/// Contains utility methods for manipulating paths.
/// </summary>
public class PathHelper
{
    /// <summary>
    /// Download a file from the specified URL to the specified destination file path.
    /// </summary>
    /// <param name="fileUrl">The URL of the file to download.</param>
    /// <param name="destinationFilePath">The path to save the file on disk, including file name.</param>
    /// <returns>A task that represents the current async operation.</returns>
    public static async Task DownloadFileAsync(string fileUrl, string destinationFilePath)
    {
        var logger = AuLogger.GetCurrentLogger<PathHelper>();
        logger.Info($"Downloading file from {fileUrl} to {destinationFilePath}");

        using HttpClient client = new();
        using var response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength;
        const int bufferSize = 81920;
        const int percentStep = 5;
        const long bytesStep = 5 * 1024 * 1024;

        var buffer = new byte[bufferSize];
        long totalBytesRead = 0;
        var nextPercentToLog = percentStep;
        var nextBytesToLog = bytesStep;

        await using var contentStream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = new FileStream(
            destinationFilePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize,
            useAsync: true);

        int bytesRead;
        while ((bytesRead = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            totalBytesRead += bytesRead;

            if (totalBytes is > 0)
            {
                var percent = (int)(totalBytesRead * 100 / totalBytes.Value);
                if (percent < nextPercentToLog) continue;

                logger.Info($"Download progress: {percent}% ({totalBytesRead:N0}/{totalBytes.Value:N0} bytes)");
                while (nextPercentToLog <= percent)
                {
                    nextPercentToLog += percentStep;
                }
            }
            else if (totalBytesRead >= nextBytesToLog)
            {
                logger.Info($"Download progress: {totalBytesRead / (1024d * 1024):F1} MB downloaded");
                while (nextBytesToLog <= totalBytesRead)
                {
                    nextBytesToLog += bytesStep;
                }
            }
        }

        logger.Info($"Download complete: {totalBytesRead:N0} bytes written to {destinationFilePath}");
    }

    /// <summary>
    /// Download a file from the specified URL and verify its SHA-256 hash.
    /// </summary>
    /// <param name="fileUrl">The URL of the file to download.</param>
    /// <param name="destinationFilePath">The path to save the file on disk, including file name.</param>
    /// <param name="expectedSha256Hash">The expected SHA-256 hash of the downloaded file. If <c>null</c> or empty, hash verification is skipped.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidDataException">Thrown if the SHA-256 hash of the downloaded file does not match the expected hash.</exception>
    public static async Task DownloadFileAsync(string fileUrl, string destinationFilePath, string? expectedSha256Hash)
    {
        await DownloadFileAsync(fileUrl, destinationFilePath);

        if (string.IsNullOrWhiteSpace(expectedSha256Hash)) {
            AuLogger.GetCurrentLogger<PathHelper>().Warn($"Expected SHA256 hash is null or empty. Skipping hash verification for file: {destinationFilePath}");
            return;
        }

        var actualSha256Hash = HashUtils.ComputeSha256Hash(destinationFilePath, false);
        if (HashUtils.CompareSha256Hash(expectedSha256Hash, actualSha256Hash)) return;

        File.Delete(destinationFilePath);
        throw new InvalidDataException($"SHA256 hash mismatch for file: {destinationFilePath}. Expected: {expectedSha256Hash}, Actual: {actualSha256Hash}. The downloaded file has been deleted.");
    }

    /// <summary>
    /// Download an image from the specified URL and convert it to a <see cref="Bitmap"/>.
    /// </summary>
    /// <param name="imageUrl">The URL of the image to download.</param>
    /// <returns>A task, that when completed, contains the downloaded image as a bitmap or the default missing image if the image could not be downloaded.</returns>
    public static async Task<Bitmap> DownloadImageAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return ConvertToBitmap(null);

        using HttpClient client = new();
        using var response = await client.GetAsync(imageUrl, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var contentStream = await response.Content.ReadAsStreamAsync();
        var image = await Image.LoadAsync<Rgba32>(contentStream);
        return ConvertToBitmap(image);
    }

    /// <summary>
    /// Convert a <see cref="Image"/> to a <see cref="Bitmap"/>.
    /// </summary>
    /// <param name="image">The <see cref="Image"/> to convert.</param>
    /// <returns>If <paramref name="image"/> is <c>null</c> or an error occured, returns a bitmap of 0 size;
    /// otherwise, returns a <see cref="Bitmap"/> representing the <see cref="Image"/>.</returns>
    public static Bitmap ConvertToBitmap(Image<Rgba32>? image)
    {
        try
        {
            using MemoryStream memoryStream = new();

            image.SaveAsBmp(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new Bitmap(memoryStream);
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<PathHelper>().Error(e);
            return new Bitmap(0, 0);
        }
    }

    /// <summary>
    /// Extract a <c>.zip</c> file to the specified destination directory.
    /// </summary>
    /// <param name="zipFilePath">The fully qualified path to a <c>.zip</c> file.</param>
    /// <param name="destinationDirectory">The directory to extract the contents of the <c>.zip</c> file into.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If the <paramref name="zipFilePath"/> or the <paramref name="destinationDirectory"/> is <c>null</c> or empty.</exception>
    /// <exception cref="FileNotFoundException">If the <paramref name="zipFilePath"/> did not exist on the filesystem.</exception>
    /// <exception cref="IOException">If an entry would extract outside <paramref name="destinationDirectory"/></exception>
    public static async Task ExtractZipFileAsync(string zipFilePath, string destinationDirectory)
    {
        if (string.IsNullOrEmpty(zipFilePath)) throw new ArgumentNullException(nameof(zipFilePath));
        if (string.IsNullOrEmpty(destinationDirectory)) throw new ArgumentNullException(nameof(destinationDirectory));
        if (!File.Exists(zipFilePath)) throw new FileNotFoundException("Zip file not found.", zipFilePath);

        Directory.CreateDirectory(destinationDirectory);
        var destinationRoot = Path.GetFullPath(destinationDirectory) + Path.DirectorySeparatorChar;

        await Task.Run(() =>
        {
            using var archive = ZipArchive.OpenArchive(zipFilePath);
            foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
            {
                var entryKey = entry.Key;
                if (string.IsNullOrEmpty(entryKey)) continue;

                var destinationPath = Path.GetFullPath(Path.Combine(destinationDirectory, entryKey));
                if (!destinationPath.StartsWith(destinationRoot, StringComparison.OrdinalIgnoreCase))
                    throw new IOException($"Zip entry '{entryKey}' would extract outside '{destinationDirectory}'.");

                var destinationDir = Path.GetDirectoryName(destinationPath);

                if (string.IsNullOrEmpty(destinationDir)) continue;

                Directory.CreateDirectory(destinationDir);
                entry.WriteToFile(destinationPath,
                    new ExtractionOptions { ExtractFullPath = true, Overwrite = true });
            }
        });
    }

    /// <summary>
    /// Returns the relative path from a base path to a full path.
    /// </summary>
    /// <param name="basePath"></param>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    public static string GetRelativePath(string basePath, string fullPath)
    {
        try
        {
            // Ensure the base path ends with a directory separator
            if (!basePath.EndsWith(Path.DirectorySeparatorChar.ToString())) basePath += Path.DirectorySeparatorChar;

            Uri baseUri = new(basePath);
            Uri fullUri = new(fullPath);

            // Get relative Uri
            var relativeUri = baseUri.MakeRelativeUri(fullUri);

            // Convert to string and replace forward slashes with backslashes
            return Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', '\\');
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<PathHelper>("GetRelativePath").Error(ex, ex.Message);
            return string.Empty;
        }
    }
}