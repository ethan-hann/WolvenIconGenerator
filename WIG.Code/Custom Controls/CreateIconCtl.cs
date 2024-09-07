using System.Diagnostics;
using AetherUtils.Core.Logging;
using WolvenIconGenerator.Utility;
using Icon = WolvenIconGenerator.Models.Icon;

namespace WolvenIconGenerator.Custom_Controls;

public partial class CreateIconCtl : UserControl
{
    private Icon? _icon;
    private Icon? _previousIcon;
    private string _imagePath = string.Empty;
    private Image? _iconImage;
    private string _outputPath = string.Empty;

    private bool IsIconValid => _icon != null;

    public CreateIconCtl()
    {
        InitializeComponent();
    }

    private void CreateIconCtl_Load(object sender, EventArgs e)
    {
        IconManager.Instance.IconImportStarted += IconManagerStatusChanged;
        IconManager.Instance.IconImportFinished += IconManagerStatusChanged;
        IconManager.Instance.CliStatus += IconManagerStatusChanged;

        ResetFields();
    }

    ~CreateIconCtl()
    {
        IconManager.Instance.IconImportStarted -= IconManagerStatusChanged;
        IconManager.Instance.IconImportFinished -= IconManagerStatusChanged;
        IconManager.Instance.CliStatus -= IconManagerStatusChanged;
    }

    private void IconManagerStatusChanged(object? sender, StatusEventArgs e)
    {
        AddStatusRow(e.Message);
    }

    private void ResetFields()
    {
        _icon = null;
        _imagePath = string.Empty;
        _iconImage = null;
        _outputPath = string.Empty;

        txtAtlasName.Text = string.Empty;
        txtImportedImagePath.Text = string.Empty;
        txtOutputPath.Text = string.Empty;

        picImportedImage.Image = null;
        dgvStatus.Rows.Clear();
    }

    private void SetIconFields()
    {
        if (InvokeRequired)
        {
            Invoke(SetIconFields);
        }
        else
        {
            _previousIcon = _icon;
            txtArchivePath.Text = _previousIcon?.ArchivePath;
            txtSha256Hash.Text = _previousIcon?.Sha256HashOfArchiveFile;
            txtImagePath.Text = _previousIcon?.ImagePath;
            txtIconPath.Text = _previousIcon?.CustomIcon?.InkAtlasPath;
            txtIconPart.Text = _previousIcon?.CustomIcon?.InkAtlasPart;
            _previousIcon?.EnsureImage();
        }
    }

    private void AddStatusRow(string? status)
    {
        if (InvokeRequired)
        {
            Invoke(() => AddStatusRow(status));
        }
        else
        {
            if (status == null) return;
            dgvStatus.Rows.Add(DateTime.Now, status);
        }
    }

    private void BtnCopyImportedImagePath_Click(object sender, EventArgs e)
    {
        try
        {
            Clipboard.SetText(txtImportedImagePath.Text, TextDataFormat.Text);
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<CreateIconCtl>().Error(ex);
        }
    }

    private void PicImportedImage_DragDrop(object sender, DragEventArgs e)
    {
        _imagePath = picImportedImage.ImagePath;
        _iconImage = picImportedImage.Image;

        txtImportedImagePath.Text = _imagePath;
    }

    private void BtnChooseOutput_Click(object sender, EventArgs e)
    {
        try
        {
            if (fldgBrowserOutput.ShowDialog(this) != DialogResult.OK) return;

            _outputPath = fldgBrowserOutput.SelectedPath;
            txtOutputPath.Text = Path.Combine(_outputPath, $"{txtAtlasName.Text}.archive");
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<CreateIconCtl>().Error(ex);
        }
    }

    private void btnCreateIcon_Click(object sender, EventArgs e)
    {
        if (bgCreateIconWorker.CancellationPending || bgCreateIconWorker.IsBusy) return;

        if (_imagePath == string.Empty)
        {
            MessageBox.Show("Please drag-and-drop an image to create the icon from.", "No Image Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (txtAtlasName.Text == string.Empty)
        {
            MessageBox.Show("Please enter an atlas name.", "No Atlas Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_outputPath == string.Empty)
        {
            MessageBox.Show("Please choose an output path.", "No Output Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btnCreateIcon.Enabled = false;
        AuLogger.GetCurrentLogger<CreateIconCtl>().Info($"Creating new icon {txtAtlasName.Text}.archive...");
        bgCreateIconWorker.RunWorkerAsync();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        IconManager.Instance.CancelOperation();
        AddStatusRow("Creation of icon requested to be cancelled.");
    }

    private void btnSendToCRA_Click(object sender, EventArgs e)
    {
        if (!IsIconValid) return;
        if (!CraIntegration.IsCraRunning())
        {
            MessageBox.Show("CRA is not running.", "Communication Failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            AuLogger.GetCurrentLogger<CreateIconCtl>().Error("CRA is not running.");
            return;
        }

        try
        {
            CraIntegration.SendIconToCra(_icon);
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<CreateIconCtl>().Error(ex);
        }
    }

    private void btnOpenContainingFolder_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtOutputPath.Text == string.Empty) return;
            var path = Path.GetDirectoryName(txtOutputPath.Text);
            if (path == null) return;

            Process.Start(path);
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<CreateIconCtl>().Error(ex);
        }
    }

    private void bgCreateIconWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
        Invoke(() =>
        {
            grpAfterCreation.Visible = false;
            btnCancel.Enabled = true;
            btnChooseOutput.Enabled = false;
            picImportedImage.Enabled = false;
        });
        
        var icon = IconManager.Instance.ImportIconImageAsync(_imagePath, txtAtlasName.Text, _outputPath).Result;
        if (icon == null)
            AddStatusRow("Failed to import icon.");
        else
            _icon = icon;
    }

    private void bgCreateIconWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
    {

    }

    private void bgCreateIconWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
        grpAfterCreation.Visible = true;
        btnCancel.Enabled = false;
        btnCreateIcon.Enabled = true;
        btnChooseOutput.Enabled = true;
        picImportedImage.Enabled = true;

        SetIconFields();

        ResetFields();

        MessageBox.Show("Icon creation finished. The icon's properties are shown until a new icon is created.", "Icon Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}