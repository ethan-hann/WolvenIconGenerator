using AetherUtils.Core.Logging;
using WolvenIconGenerator.Utility;
using Icon = WolvenIconGenerator.Models.Icon;

namespace WolvenIconGenerator.Custom_Controls
{
    public partial class CreateIconCtl : UserControl
    {
        private readonly Icon _icon;

        public CreateIconCtl()
        {
            InitializeComponent();

            _icon = new Icon();
        }

        private void btnSelectImageFromDisk_Click(object sender, EventArgs e)
        {
            if (CraIntegration.IsCraRunning())
            {
                CraIntegration.SendIconToCra(_icon);
            }
        }

        private void btnCopyImportedImagePath_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtImportedImagePath.Text, TextDataFormat.Text);
                CraIntegration.SendIconToCra(_icon);
            }
            catch (Exception ex)
            {
                AuLogger.GetCurrentLogger<CreateIconCtl>().Error(ex);
            }
        }

        private void picImportedImage_DragDrop(object sender, DragEventArgs e)
        {
            _icon.ImagePath = picImportedImage.ImagePath;
            _icon.EnsureImage();

            picImportedImage.Image = _icon.IconImage;
            txtImportedImagePath.Text = _icon.ImagePath;
        }
    }
}
