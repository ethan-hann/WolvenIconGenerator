using AetherUtils.Core.Logging;
using System.ComponentModel;
using System.Diagnostics;
using WIG.Lib.Models;
using WIG.Lib.Utility;
using WolvenIconGenerator.Utility;

namespace WolvenIconGenerator.Custom_Controls;

public partial class IconCreator : UserControl
{
    private int _currentTabIndex = 0;
    private WolvenIcon? _icon;
    private WolvenIcon? _previousIcon;
    private string _imagePath = string.Empty;
    private Image? _iconImage;
    private string _outputPath = string.Empty;

    private bool IsIconValid => _previousIcon != null;

    public IconCreator()
    {
        InitializeComponent();
    }

    private void IconCreator_Load(object sender, EventArgs e)
    {
        IconManager.Instance.IconImportStarted += IconManagerStatusChanged;
        IconManager.Instance.IconImportFinished += IconManagerStatusChanged;
        IconManager.Instance.CliStatus += IconManagerStatusChanged;

        ResetFields();
    }

    ~IconCreator()
    {
        IconManager.Instance.IconImportStarted -= IconManagerStatusChanged;
        IconManager.Instance.IconImportFinished -= IconManagerStatusChanged;
        IconManager.Instance.CliStatus -= IconManagerStatusChanged;
    }

    private void IconManagerStatusChanged(object? sender, StatusEventArgs e)
    {
        Invoke(() =>
        {
            AddStatusRow(e.Message);
            SetProgressPercentage(e.ProgressPercentage);
        });
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

    private void SetProgressPercentage(int percentage)
    {
        if (InvokeRequired)
        {
            Invoke(() => SetProgressPercentage(percentage));
        }
        else
        {
            pgIconProgress.Value = percentage;
        }
    }

    private void SelectNextTab()
    {
        _currentTabIndex++;
        if (_currentTabIndex >= iconTabs.TabCount)
            _currentTabIndex = iconTabs.TabCount - 1;
        iconTabs.SelectedIndex = _currentTabIndex;
    }

    private void SelectPreviousTab()
    {
        _currentTabIndex--;
        if (_currentTabIndex <= 0)
            _currentTabIndex = 0;
        iconTabs.SelectedIndex = _currentTabIndex;
    }

    private void BtnPreviousPage1_Click(object sender, EventArgs e)
    {
        SelectPreviousTab();
    }

    private void BtnNextPage1_Click(object sender, EventArgs e)
    {
        SelectNextTab();
    }

    private void BtnNextPage2_Click(object sender, EventArgs e)
    {
        SelectNextTab();
    }

    private void BtnNextPage3_Click(object sender, EventArgs e)
    {
        SelectNextTab();
    }

    private void BtnPreviousPage2_Click(object sender, EventArgs e)
    {
        SelectPreviousTab();
    }

    private void BtnPreviousPage3_Click(object sender, EventArgs e)
    {
        SelectPreviousTab();
    }

    private void BtnCopyImportedImagePath_Click(object sender, EventArgs e)
    {
        try
        {
            Clipboard.SetText(txtImportedImagePath.Text, TextDataFormat.Text);
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<IconCreator>().Error(ex);
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
            AuLogger.GetCurrentLogger<IconCreator>().Error(ex);
        }
    }

    private void BtnCreateIcon_Click(object sender, EventArgs e)
    {
        if (bgCreateIconWorker.CancellationPending || bgCreateIconWorker.IsBusy) return;

        if (_imagePath == string.Empty)
        {
            MessageBox.Show("Please drag-and-drop an image to create the icon from.", "No Image Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            iconTabs.SelectedIndex = 0;
            return;
        }

        if (txtAtlasName.Text == string.Empty)
        {
            MessageBox.Show("Please enter an atlas name.", "No Atlas Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            iconTabs.SelectedIndex = 1;
            return;
        }

        if (_outputPath == string.Empty)
        {
            MessageBox.Show("Please choose an output path.", "No Output Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            iconTabs.SelectedIndex = 1;
            return;
        }

        if (_previousIcon != null && CraIntegration.IsCraRunning())
        {
            var result = MessageBox.Show("A previous icon has been created. If it hasn't been sent to CRA, you will have to add the icon to the station manually. Continue?", "Overwrite Icon", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No) return;
        }

        btnCreateIcon.Enabled = false;
        AuLogger.GetCurrentLogger<IconCreator>().Info($"Creating new icon {txtAtlasName.Text}.archive...");
        bgCreateIconWorker.RunWorkerAsync();
    }

    private void BtnCancel_Click(object sender, EventArgs e)
    {
        IconManager.Instance.CancelOperation();
        AddStatusRow("Creation of icon requested to be cancelled.");
    }

    private void BtnSendToCRA_Click(object sender, EventArgs e)
    {
        if (!IsIconValid) return;

        if (!CraIntegration.IsCraRunning())
        {
            MessageBox.Show("CRA is not running.", "Communication Failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            AuLogger.GetCurrentLogger<IconCreator>().Error("CRA is not running.");
            return;
        }

        try
        {
            CraIntegration.SendIconToCra(_icon);
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<IconCreator>().Error(ex);
        }
    }

    private void BtnOpenContainingFolder_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtArchivePath.Text == string.Empty) return;
            var path = Path.GetDirectoryName(txtArchivePath.Text);
            if (path == null) return;

            Process.Start(path);
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<IconCreator>().Error(ex);
        }
    }

    private void BgCreateIconWorker_DoWork(object sender, DoWorkEventArgs e)
    {
        Invoke(() =>
        {
            tabIconPropAndActions.Visible = false;
            btnCancel.Enabled = true;
            btnChooseOutput.Enabled = false;
            picImportedImage.Enabled = false;
            txtAtlasName.Enabled = false;
            SetProgressPercentage(0);
        });

        var icon = IconManager.Instance.GenerateIconImageAsync(_imagePath, txtAtlasName.Text).Result;
        if (icon == null)
            AddStatusRow("Failed to import icon.");
        else
        {
            _icon = icon;
            if (icon.ArchivePath != null)
                File.Copy(icon.ArchivePath, txtOutputPath.Text);
        }
    }

    private void BgCreateIconWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        btnCancel.Enabled = false;
        btnCreateIcon.Enabled = true;
        btnChooseOutput.Enabled = true;
        picImportedImage.Enabled = true;
        txtAtlasName.Enabled = true;
        iconTabs.SelectedIndex = 3; // Show the icon properties tab
        SetProgressPercentage(100);

        SetIconFields();
        ResetFields();

        MessageBox.Show("Icon creation finished. The icon's properties are shown until a new icon is created.", "Icon Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BtnCopyImagePath_Click(object sender, EventArgs e)
    {
        try
        {
            Clipboard.SetText(txtImagePath.Text, TextDataFormat.Text);
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<IconCreator>().Error(ex);
        }
    }

    private void BtnCopyArchivePath_Click(object sender, EventArgs e)
    {
        try
        {
            Clipboard.SetText(txtArchivePath.Text, TextDataFormat.Text);
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<IconCreator>().Error(ex);
        }
    }

    private void BtnCopySha256Hash_Click(object sender, EventArgs e)
    {
        try
        {
            Clipboard.SetText(txtSha256Hash.Text, TextDataFormat.Text);
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<IconCreator>().Error(ex);
        }
    }

    private void BtnCopyIconPath_Click(object sender, EventArgs e)
    {
        try
        {
            Clipboard.SetText(txtIconPath.Text, TextDataFormat.Text);
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<IconCreator>().Error(ex);
        }
    }

    private void BtnCopyIconPart_Click(object sender, EventArgs e)
    {
        try
        {
            Clipboard.SetText(txtIconPart.Text, TextDataFormat.Text);
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<IconCreator>().Error(ex);
        }
    }
}