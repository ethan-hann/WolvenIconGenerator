using AetherUtils.Core.Logging;
using WolvenIconGenerator.Utility;

namespace WolvenIconGenerator.Custom_Controls;

public partial class CustomPictureBox : PictureBox
{
    public string ImagePath { get; private set; } = string.Empty;
    public CustomPictureBox()
    {
        AllowDrop = true;
        DragEnter += CustomPictureBox_DragEnter;
        DragDrop += CustomPictureBox_DragDrop;
    }

    public sealed override bool AllowDrop
    {
        get => base.AllowDrop;
        set => base.AllowDrop = value;
    }

    private void CustomPictureBox_DragEnter(object? sender, DragEventArgs e)
    {
        e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) is true
            ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void CustomPictureBox_DragDrop(object? sender, DragEventArgs e)
    {
        try
        {
            var data = e.Data;
            if (data == null || !data.GetDataPresent(DataFormats.FileDrop)) return;

            var files = data.GetData(DataFormats.FileDrop) as string[];
            if (!(files?.Length > 0)) return;

            var file = files[0];
            var image = ImageUtils.LoadImage(file);
            if (image == null) return;

            Image = image;
            ImagePath = file;
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<CustomPictureBox>().Error(ex.Message);
        }
    }
}