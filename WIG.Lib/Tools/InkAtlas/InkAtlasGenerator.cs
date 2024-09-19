using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AetherUtils.Core.Files;
using WIG.Lib.Utility;
using Color = SixLabors.ImageSharp.Color;
using Image = SixLabors.ImageSharp.Image;
using Point = SixLabors.ImageSharp.Point;

namespace WIG.Lib.Tools.InkAtlas;

internal class InkAtlasGenerator
{
    public event EventHandler<string?>? OutputChanged;
    public event EventHandler<string?>? ErrorChanged;

    private readonly Json<InkAtlasData> _jsonData = new();

    /// <summary>
    /// Generate an .inkatlas.json file and combined image from a folder of images.
    /// </summary>
    /// <param name="iconFolderPath">The path to the folder containing the .png files.</param>
    /// <param name="outputFolderPath">The output path where the final .inkatlas.json file should be saved.</param>
    /// <param name="atlasName">The name of the atlas to generate.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task GenerateInkAtlasJsonAsync(string iconFolderPath, string outputFolderPath, string atlasName)
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

        await ProcessImagesAndGenerateInkAtlas(outputFolderPath, atlasName, pngFiles);

        OnOutputChanged($"InkAtlas generation complete for {atlasName}.");
    }

    private async Task ProcessImagesAndGenerateInkAtlas(string outputFolderPath, string atlasName, string[] pngFiles)
    {
        var images = new List<ImageData>();

        foreach (var pngFile in pngFiles)
        {
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
        }

        await CombineImagesAndGenerateJson(images, outputFolderPath, atlasName);
    }

    private async Task CombineImagesAndGenerateJson(List<ImageData> images, string outputFolderPath, string atlasName)
    {
        var maxWidth = 2048;
        var grid = OrganizeImagesIntoGrid(images, maxWidth);
        var totalWidth = CalculateTotalWidth(grid, maxWidth);
        var totalHeight = CalculateTotalHeight(grid);

        using var combinedImage = new Image<Rgba32>(totalWidth, totalHeight);
        combinedImage.Mutate(ctx => ctx.BackgroundColor(Color.Transparent));  // Set background to transparent

        var jsonData = CreateAtlasJson(grid, combinedImage, totalWidth, totalHeight, outputFolderPath, atlasName);

        await SaveImagesAndJsonAsync(combinedImage, outputFolderPath, atlasName, totalWidth, totalHeight, jsonData);
    }

    private List<List<ImageData>> OrganizeImagesIntoGrid(List<ImageData> images, int maxWidth)
    {
        var grid = new List<List<ImageData>>();
        var currentRow = new List<ImageData>();
        int currentWidth = 0;
        int maxHeightInRow = 0;

        foreach (var imageData in images)
        {
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
        }

        if (currentRow.Any()) grid.Add(currentRow);

        return grid;
    }

    private int CalculateTotalWidth(List<List<ImageData>> grid, int maxWidth)
    {
        int totalWidth = 0;
        foreach (var row in grid)
        {
            int rowWidth = row.Sum(img => img.Image.Width + 1);
            totalWidth = Math.Max(totalWidth, rowWidth);
        }
        return Math.Min(totalWidth, maxWidth);
    }

    private int CalculateTotalHeight(List<List<ImageData>> grid)
    {
        return grid.Sum(row => row.Max(img => img.Image.Height)) + (grid.Count - 1);
    }

    private InkAtlasData CreateAtlasJson(List<List<ImageData>> grid, Image<Rgba32> combinedImage, int totalWidth, int totalHeight, string outputFolder, string atlasName)
    {
        var jsonData = new InkAtlasData
        {
            Header = new Header(),
            Data = new Data()
        };

        //Add the depot path values
        jsonData.Data.RootChunk.Slots.Elements[0].Texture.DepotPath.Value = $"{atlasName}.xbm";
        jsonData.Data.RootChunk.Slots.Elements[1].Texture.DepotPath.Value = $"{atlasName}_1080.xbm";

        int currentY = 0;
        foreach (var row in grid)
        {
            // Ensure the row is not null or empty
            if (row == null || !row.Any())
            {
                OnErrorChanged("Error: Encountered an empty or null row in the grid.");
                continue; // Skip this row to avoid further issues
            }

            int maxHeightInRow = row.Max(img => img.Image.Height);
            int currentX = 0;

            foreach (var imageData in row)
            {
                // Ensure imageData is not null
                if (imageData?.Image == null)
                {
                    OnErrorChanged("Error: Encountered a null image in the row.");
                    continue; // Skip this image to avoid further issues
                }

                int topPixel = currentY + (maxHeightInRow - imageData.Image.Height) / 2;

                combinedImage.Mutate(ctx => ctx.DrawImage(imageData.Image, new Point(currentX, topPixel), 1));

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
                    PartName = new PartName { Type = "CName", Storage = "string", Value = atlasName }
                };

                //Add 1 pixel spacing between images
                currentX += imageData.Image.Width + 1;

                // Safely add the part data to both Elements[0] and Elements[1]
                if (jsonData.Data.RootChunk.Slots.Elements.Count >= 2)
                {
                    jsonData.Data.RootChunk.Slots.Elements[0].Parts.Add(partData);
                    jsonData.Data.RootChunk.Slots.Elements[1].Parts.Add(partData);
                }
                else
                {
                    OnErrorChanged("Error: No slots available in JSON structure to add part data.");
                }
            }

            currentY += maxHeightInRow + 1; //Add 1 pixel spacing between rows
        }

        return jsonData;
    }

    private async Task SaveImagesAndJsonAsync(Image<Rgba32> combinedImage, string outputFolderPath, string atlasName, int totalWidth, int totalHeight, InkAtlasData jsonData)
    {
        if (!Directory.Exists(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
        }

        string combinedImagePath = Path.Combine(outputFolderPath, $"{atlasName}.png");
        await combinedImage.SaveAsPngAsync(combinedImagePath);
        OnOutputChanged($"Combined image saved to {combinedImagePath}");

        string resizedImagePath = combinedImagePath.Replace(".png", "_1080.png");
        using var resizedImage = combinedImage.Clone(ctx => ctx.Resize(totalWidth / 2, totalHeight / 2));
        await resizedImage.SaveAsPngAsync(resizedImagePath);
        OnOutputChanged($"Resized image saved to {resizedImagePath}");

        // Use your custom JSON class to save the JSON data
        var jsonOutputPath = Path.Combine(outputFolderPath, $"{atlasName}.inkatlas.json");
        var jsonSaved = _jsonData.SaveJson(jsonOutputPath, jsonData);
        if (jsonSaved)
            OnOutputChanged($"The .inkatlas data was saved to {jsonOutputPath}");
        else
            OnErrorChanged("Error saving .inkatlas data.");
    }

    private void OnOutputChanged(string? output)
    {
        if (!string.IsNullOrEmpty(output))
        {
            OutputChanged?.Invoke(this, output);
        }
    }

    private void OnErrorChanged(string? error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            ErrorChanged?.Invoke(this, error);
        }
    }
}