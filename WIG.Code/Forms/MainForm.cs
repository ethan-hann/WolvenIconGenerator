using AetherUtils.Core.Logging;
using System.Diagnostics;
using WolvenIconGenerator.Custom_Controls;
using WolvenIconGenerator.Properties;
using WolvenIconGenerator.Utility;

namespace WolvenIconGenerator.Forms;

public partial class MainForm : Form
{
    private readonly ImageList _tabImages = new();

    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        CreateImageList();
        ResetTabs();
    }

    private void CreateImageList()
    {
        _tabImages.Images.Add("create", Resources.add_image);
        _tabImages.Images.Add("extract", Resources.export);
        _tabImages.ImageSize = new Size(16, 16);
        mainTabs.ImageList = _tabImages;

        tabCreateIcon.ImageKey = @"create";
        tabExtractIcon.ImageKey = @"extract";
    }

    private void ResetTabs()
    {
        ResetCreateTab();
        ResetExportTab();

        mainTabs.SelectedTab = tabCreateIcon;
    }

    private void ResetCreateTab()
    {
        tabCreateIcon.Controls.Clear();
        tabCreateIcon.Controls.Add(new IconCreator { Dock = DockStyle.Fill });
    }

    private void ResetExportTab()
    {
        tabExtractIcon.Controls.Clear();
        tabExtractIcon.Controls.Add(new ExportIconCtl { Dock = DockStyle.Fill });
    }

    private void checkCRAConnectionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var isCraRunning = CraIntegration.IsCraRunning();
        MessageBox.Show(isCraRunning ? "CRA is running and can be reached." : "CRA is not running.",
            isCraRunning ? "Communication Successful" : "Communication Failure",
            MessageBoxButtons.OK, isCraRunning ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }

    private void createIconToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ResetCreateTab();
    }

    private void exportIconToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ResetExportTab();
    }

    private void openLogsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Wolven Icon Generator", "Logs");
            var startInfo = new ProcessStartInfo
            {
                FileName = logPath,
                UseShellExecute = true,
                Verb = "open"
            };

            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred while trying to open the logs folder.\n\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            AuLogger.GetCurrentLogger<MainForm>().Error(ex, "Error while trying to open the logs folder.");
        }
    }
}