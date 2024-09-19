using AetherUtils.Core.Configuration;
using AetherUtils.Core.Logging;
using System.Reflection;
using WIG.Lib.Utility;

namespace WolvenIconGenerator.Forms;

public partial class SplashScreen : Form
{
    public SplashScreen()
    {
        InitializeComponent();

        var version = Assembly.GetExecutingAssembly().GetName()?.Version;

        SetVersionLabel(version);
    }

    /// <summary>
    /// Handles the Load event of the splash screen. Starts the background tasks.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private async void SplashScreen_Load(object sender, EventArgs e)
    {
        UpdateStatus("Starting...");
        var statusMessages = await PerformBackgroundTasks();
        statusMessages.AddRange(SetUpLogger());

        statusMessages.ForEach(msg =>
        {
            if (msg.Contains("Error", StringComparison.CurrentCultureIgnoreCase))
                AuLogger.GetCurrentLogger<SplashScreen>().Error(msg);
            else
                AuLogger.GetCurrentLogger<SplashScreen>().Info(msg);
        });
        Close();
    }

    /// <summary>
    /// Performs background tasks such as migrating settings and checking for updates.
    /// </summary>
    /// <returns>A list of status messages generated during the tasks.</returns>
    private async Task<List<string>> PerformBackgroundTasks()
    {
        var statusMessages = new List<string>();


        // Check for updates if needed
        //UpdateStatus("Checking for updates...");
        //await Task.Run(Updater.CheckForUpdates);

        //Setup Icon Manager
        UpdateStatus("Setting up Icon Manager...");
        await IconManager.Instance.Initialize();
        await Task.Delay(500); // Simulate delay

        statusMessages.Add(IconManager.Instance.IsInitialized
            ? "Icon Manager initialized successfully."
            : "Icon Manager initialization failed.");

        // Simulate delay before finalizing
        UpdateStatus("Finalizing...");
        await Task.Delay(1000);

        return statusMessages;
    }

    /// <summary>
    /// Updates the status message on the splash screen.
    /// </summary>
    /// <param name="message">The status message to display.</param>
    public void UpdateStatus(string message)
    {
        if (InvokeRequired)
            Invoke(new Action<string>(UpdateStatus), [message]);
        else
            lblStatus.Text = message;
    }

    /// <summary>
    /// Sets the version label on the splash screen.
    /// </summary>
    /// <param name="version">The version to display.</param>
    private void SetVersionLabel(Version? version)
    {
        if (InvokeRequired)
            Invoke(new Action<Version?>(SetVersionLabel), [version]);
        else
            lblVersion.Text =
                version != null ? @$"{version.Major}.{version.Minor}.{version.Build}" : @"Version Unknown";
    }

    private List<string> SetUpLogger()
    {
        var logOptions = new LogOptions()
        {
            AppName = "Wolven Icon Generator",
            LogFileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Wolven Icon Generator", "Logs"),
            NewFileEveryLaunch = true,
            IncludeDateTime = true,
            LogHeader = $"Wolven Icon Generator - {lblVersion.Text}"
        };

        AuLogger.Initialize(logOptions);
        return AuLogger.IsInitialized ? ["Logger initialized successfully."] : ["Failed to initialize logger."];
    }
}