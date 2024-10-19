// InkAtlasGenerator.cs : WIG.Lib
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

using AetherUtils.Core.Files;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;
using Image = SixLabors.ImageSharp.Image;
using Point = SixLabors.ImageSharp.Point;

namespace WIG.Lib.Tools.InkAtlas;

internal class InkAtlasGenerator
{
    private readonly Json<InkAtlasData> _jsonData = new();
    public event EventHandler<string?>? OutputChanged;
    public event EventHandler<string?>? ErrorChanged;

    /// <summary>
    /// Generate an .inkatlas.json file and combined image from a folder of images.
    /// Supports progress reporting and cancellation.
    /// </summary>
    /// <param name="iconFolderPath">The path to the folder containing the .png files.</param>
    /// <param name="outputFolderPath">The output path where the final .inkatlas.json file should be saved.</param>
    /// <param name="atlasName">The name of the atlas to generate.</param>
    /// <param name="token">A cancellation token to cancel the task.</param>
    /// <param name="progress">An optional progress reporter to track progress.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task GenerateInkAtlasJsonAsync(string iconFolderPath, string outputFolderPath, string atlasName,
        CancellationToken token, IProgress<int>? progress = null)
    {
        iconFolderPath = iconFolderPath.Trim('"', '\'');
        outputFolderPath = outputFolderPath.Trim('"', '\'');

        if (!Directory.Exists(iconFolderPath))
        {
            OnErrorChanged("Error: The entered path does not exist.");
            return;
        }

        var pngFiles = Directory.GetFiles(iconFolderPath, "*.png");
        if (pngFiles.Length == 0)
        {
            OnErrorChanged("Error: The entered folder does not contain any PNG files.");
            return;
        }

        await ProcessImagesAndGenerateInkAtlas(outputFolderPath, atlasName, pngFiles, token, progress);

        OnOutputChanged($"InkAtlas generation complete for {atlasName}.");
    }

    private async Task ProcessImagesAndGenerateInkAtlas(string outputFolderPath, string atlasName, string[] pngFiles,
        CancellationToken token, IProgress<int>? progress)
    {
        var images = new List<ImageData>();

        foreach (var pngFile in pngFiles)
        {
            token.ThrowIfCancellationRequested(); // Check if cancellation was requested

            try
            {
                if (!File.Exists(pngFile))
                {
                    OnErrorChanged($"File not found: {pngFile}");
                    continue;
                }

                if (!pngFile.ToLower().EndsWith(".png"))
                {
                    OnErrorChanged($"Invalid file format: {pngFile} (expected .png)");
                    continue;
                }

                var img = Image.Load<Rgba32>(pngFile);

                if (img.Width == 0 || img.Height == 0)
                {
                    OnErrorChanged($"Invalid image file: {pngFile}");
                    continue;
                }

                images.Add(new ImageData { Image = img, Name = Path.GetFileNameWithoutExtension(pngFile) });
            }
            catch (ImageFormatException ex)
            {
                OnErrorChanged($"ImageFormatException: Error opening image {pngFile}: {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                OnErrorChanged($"Error opening image {pngFile}: {ex.Message}");
                return;
            }

            // Report progress after each image is processed
            progress?.Report(images.Count * 100 / pngFiles.Length);
        }

        await CombineImagesAndGenerateJson(images, outputFolderPath, atlasName, token, progress);
    }

    private async Task CombineImagesAndGenerateJson(List<ImageData> images, string outputFolderPath, string atlasName,
        CancellationToken token, IProgress<int>? progress)
    {
        const int maxWidth = 2048;
        var grid = OrganizeImagesIntoGrid(images, maxWidth);
        var totalWidth = CalculateTotalWidth(grid, maxWidth);
        var totalHeight = CalculateTotalHeight(grid);

        using var combinedImage = new Image<Rgba32>(totalWidth, totalHeight);
        combinedImage.Mutate(ctx => ctx.BackgroundColor(Color.Transparent)); // Set background to transparent

        var jsonData = CreateAtlasJson(grid, combinedImage, totalWidth, totalHeight, atlasName);

        await SaveImagesAndJsonAsync(combinedImage, outputFolderPath, atlasName, jsonData, token, progress);
    }

    private List<List<ImageData>> OrganizeImagesIntoGrid(List<ImageData> images, int maxWidth)
    {
        var grid = new List<List<ImageData>>();
        var currentRow = new List<ImageData>();
        var currentWidth = 0;
        var maxHeightInRow = 0;

        foreach (var imageData in images)
            if (currentWidth + imageData.Image.Width <= maxWidth)
            {
                currentRow.Add(imageData);
                currentWidth += imageData.Image.Width + 1;
                maxHeightInRow = Math.Max(maxHeightInRow, imageData.Image.Height);
            }
            else
            {
                grid.Add(currentRow);
                currentRow = new List<ImageData> { imageData };
                currentWidth = imageData.Image.Width + 1;
                maxHeightInRow = imageData.Image.Height;
            }

        if (currentRow.Any()) grid.Add(currentRow);

        return grid;
    }

    private int CalculateTotalWidth(List<List<ImageData>> grid, int maxWidth)
    {
        var totalWidth = grid.Select(row => row.Sum(img => img.Image.Width + 1)).Prepend(0).Max();
        return Math.Min(totalWidth, maxWidth);
    }

    private int CalculateTotalHeight(List<List<ImageData>> grid)
    {
        return grid.Sum(row => row.Max(img => img.Image.Height)) + (grid.Count - 1);
    }

    private InkAtlasData CreateAtlasJson(List<List<ImageData>> grid, Image<Rgba32> combinedImage, int totalWidth,
        int totalHeight, string atlasName)
    {
        var jsonData = new InkAtlasData
        {
            Header = new Header(),
            Data = new Data()
        };

        //Add the depot path values
        jsonData.Data.RootChunk.Slots.Elements[0].Texture.DepotPath.Value = $@"base\icon\{atlasName}.xbm";

        var currentY = 0;
        foreach (var row in grid)
        {
            var maxHeightInRow = row.Max(img => img.Image.Height);
            var currentX = 0;

            foreach (var imageData in row)
            {
                var topPixel = currentY + (maxHeightInRow - imageData.Image.Height) / 2;

                var x = currentX;
                combinedImage.Mutate(ctx => ctx.DrawImage(imageData.Image, new Point(x, topPixel), 1));

                var partData = new InkTextureAtlasMapper
                {
                    Type = "inkTextureAtlasMapper",
                    ClippingRectInPixels = new ClippingRectInPixels
                    {
                        Type = "Rect",
                        Bottom = topPixel + imageData.Image.Height,
                        Left = currentX,
                        Right = currentX + imageData.Image.Width,
                        Top = topPixel
                    },
                    ClippingRectInUvCoords = new ClippingRectInUvCoords
                    {
                        Type = "RectF",
                        Bottom = (float)(topPixel + imageData.Image.Height) / totalHeight,
                        Left = (float)currentX / totalWidth,
                        Right = (float)(currentX + imageData.Image.Width) / totalWidth,
                        Top = (float)topPixel / totalHeight
                    },
                    PartName = new PartName { Type = "CName", Storage = "string", Value = "icon_part" }
                };

                currentX += imageData.Image.Width + 1;

                if (jsonData.Data.RootChunk.Slots.Elements.Count == 1)
                    jsonData.Data.RootChunk.Slots.Elements[0].Parts.Add(partData);
                else
                    OnErrorChanged("Error: No slots available in JSON structure to add part data.");
            }

            currentY += maxHeightInRow + 1;
        }

        return jsonData;
    }

    private async Task SaveImagesAndJsonAsync(Image<Rgba32> combinedImage, string outputFolderPath, string atlasName,
        InkAtlasData jsonData, CancellationToken token, IProgress<int>? progress)
    {
        if (!Directory.Exists(outputFolderPath)) Directory.CreateDirectory(outputFolderPath);

        var combinedImagePath = Path.Combine(outputFolderPath, $"{atlasName}.png");
        await combinedImage.SaveAsPngAsync(combinedImagePath, token);
        OnOutputChanged($"Combined image saved to {combinedImagePath}");

        // Use custom JSON class to save the JSON data
        var jsonOutputPath = Path.Combine(outputFolderPath, $"{atlasName}.inkatlas.json");
        var jsonSaved = _jsonData.SaveJson(jsonOutputPath, jsonData);
        if (jsonSaved)
            OnOutputChanged($"The .inkatlas data was saved to {jsonOutputPath}");
        else
            OnErrorChanged("Error saving .inkatlas data.");

        // Report final progress as 100%
        progress?.Report(100);
    }

    private void OnOutputChanged(string? output)
    {
        if (!string.IsNullOrEmpty(output)) OutputChanged?.Invoke(this, output);
    }

    private void OnErrorChanged(string? error)
    {
        if (!string.IsNullOrEmpty(error)) ErrorChanged?.Invoke(this, error);
    }
}