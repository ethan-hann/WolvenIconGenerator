using AetherUtils.Core.Configuration;
using AetherUtils.Core.Logging;
using WolvenIconGenerator.Custom_Controls;
using WolvenIconGenerator.Properties;
using WolvenIconGenerator.Utility;

namespace WolvenIconGenerator
{
    public partial class MainForm : Form
    {
        private readonly ImageList _tabImages = new();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetUpLogger();
            CreateImageList();
            ResetTabs();
        }

        private void SetUpLogger()
        {
            var logOptions = new LogOptions()
            {
                AppName = "Wolven Icon Generator",
                LogFileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Wolven Icon Generator", "Logs"),
                NewFileEveryLaunch = true,
                IncludeDateTime = true,
                LogHeader = $"Wolven Icon Generator - {ProductVersion}"
            };

            AuLogger.Initialize(logOptions);
            if (AuLogger.IsInitialized)
                AuLogger.GetCurrentLogger<MainForm>().Info("Logger initialized successfully.");
            else
                MessageBox.Show("Failed to initialize logger.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void CreateImageList()
        {
            _tabImages.Images.Add("create", Resources.add_image);
            _tabImages.Images.Add("export", Resources.export);
            _tabImages.ImageSize = new Size(16, 16);
            mainTabs.ImageList = _tabImages;

            tabCreateIcon.ImageKey = "create";
            tabExportIcon.ImageKey = "export";
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
            tabCreateIcon.Controls.Add(new CreateIconCtl { Dock = DockStyle.Fill });
        }

        private void ResetExportTab()
        {
            tabExportIcon.Controls.Clear();
            tabExportIcon.Controls.Add(new ExportIconCtl { Dock = DockStyle.Fill });
        }

        private void checkCRAConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var isCraRunning = CraIntegration.IsCraRunning();
            MessageBox.Show(isCraRunning ? "CRA is running and can be reached." : "CRA is not running.", 
                isCraRunning ? "Communication Successful" : "Communication Failure",
                MessageBoxButtons.OK, isCraRunning ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }
    }
}