using AetherUtils.Core.Logging;
using System.Reflection;
using WIG.Lib.Models;

namespace WIG.Lib.Utility;

/// <summary>
/// Manager class responsible for handling the generation and management of custom icons.
/// This class is a singleton and as such should be accessed via <see cref="Instance"/>.
/// <para>This manager can both extract icons from <c>.archive</c> files (export) and create <c>.archive</c> files from raw PNGs (import).</para>
/// <para>All long-running operations support cancellation via the <see cref="CancelOperation"/> method.</para>
/// <para>Events can be subscribed to in order to track the progress of the running operations.</para>
/// </summary>
public class IconManager : IDisposable
{
    private static readonly object Lock = new();
    private static IconManager? _instance;

    private string _wolvenKitCliDownloadUrl = string.Empty;
    private string _inkAtlasExe = string.Empty;
    private string _wolvenKitCliExe = string.Empty;
    private bool _isWolvenKitDownloaded;
    private bool _isWolvenKitExtracted;
    private CancellationTokenSource? _cancellationTokenSource;

    private Cli? _inkAtlasCli;
    private Cli? _wolvenKitCli;

    public static IconManager Instance
    {
        get
        {
            lock (Lock)
            {
                return _instance ??= new IconManager();
            }
        }
    }

    /// <summary>
    /// Event that is raised when the status of the CLI changes.
    /// </summary>
    public event EventHandler<StatusEventArgs>? CliStatus;

    #region Import Events
    public event EventHandler<StatusEventArgs>? IconImportStarted;
    public event EventHandler<StatusEventArgs>? IconImportFinished;
    #endregion

    #region Export Events
    public event EventHandler<StatusEventArgs>? IconExportStarted;
    public event EventHandler<StatusEventArgs>? IconExportFinished;
    #endregion

    #region Properties
    /// <summary>
    /// Get or set the current version of WolvenKit to download. Defaults to <c>8.14.0</c>.
    /// </summary>
    public string? WolvenKitVersion { get; set; } = "8.14.0";

    /// <summary>
    /// Get the working directory for the icon generator. Defaults to <c>%LOCALAPPDATA%\RadioExt-Helper\tools</c>.
    /// </summary>
    public string? WorkingDirectory { get; private set; }

    /// <summary>
    /// Get the temporary directory for the icon generator. Defaults to <c>%TEMP%\{random folder name}</c>.
    /// </summary>
    public string? WolvenKitTempDirectory { get; private set; }

    /// <summary>
    /// Get the path to the image import directory. Defaults to <c>%LOCALAPPDATA%\RadioExt-Helper\tools\imported</c>.
    /// <para>This directory is where .png files that have been imported are stored. The file names are GUIDs.</para>
    /// </summary>
    public string? ImageImportDirectory { get; private set; }

    /// <summary>
    /// Get a value indicating if the icon manager has been initialized.
    /// </summary>
    public bool IsInitialized { get; private set; }

    #endregion

    private IconManager()
    {
        try
        {
            if (_instance != null)
                throw new InvalidOperationException("An instance of IconManager already exists. Use IconManager.Instance to access it.");
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("Constructor").Error(e.Message);
        }
    }

    ~IconManager() => CleanupResources();

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        CleanupResources();
    }

    /// <summary>
    /// Cleans up any resources used by the icon manager. This mainly involves deleting the temporary directory: <see cref="WolvenKitTempDirectory"/>.
    /// </summary>
    private void CleanupResources()
    {
        try
        {
            if (Directory.Exists(WolvenKitTempDirectory))
                Directory.Delete(WolvenKitTempDirectory, true);
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("CleanupResources").Error(e.Message);
        }
    }

    /// <summary>
    /// Initialize the icon manager. This method will set up the required paths and download the WolvenKit CLI if required. Only
    /// needs to be called once.
    /// </summary>
    public async Task Initialize()
    {
        if (IsInitialized) return;

        try
        {
            SetupRequiredPaths();

            _wolvenKitCliDownloadUrl = $"https://github.com/WolvenKit/WolvenKit/releases/download/{WolvenKitVersion}/WolvenKit.Console-{WolvenKitVersion}.zip";
            _inkAtlasExe = Assembly.GetExecutingAssembly().ExtractEmbeddedResource("WolvenIconGenerator.Tools.InkAtlas.generate_inkatlas.exe");

            if (WolvenKitTempDirectory == null)
                throw new InvalidOperationException("The WolvenKit temp directory is null.");

            _wolvenKitCliExe = Path.Combine(WolvenKitTempDirectory, "WolvenKit.CLI.exe");

            await DownloadWolvenKitIfRequiredAsync();

            if (!File.Exists(_inkAtlasExe))
                throw new FileNotFoundException("The inkatlas executable could not be found.", _inkAtlasExe);

            if (!File.Exists(_wolvenKitCliExe))
                throw new FileNotFoundException("The WolvenKit CLI executable could not be found.", _wolvenKitCliExe);

            _inkAtlasCli = new Cli(_inkAtlasExe);
            _inkAtlasCli.ErrorChanged += InkAtlasCli_ErrorChanged;
            _inkAtlasCli.OutputChanged += InkAtlasCli_OutputChanged;

            _wolvenKitCli = new Cli(_wolvenKitCliExe);
            _wolvenKitCli.ErrorChanged += WolvenKitCli_ErrorChanged;
            _wolvenKitCli.OutputChanged += WolvenKitCli_OutputChanged;

            IsInitialized = true;
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("Initialize").Error(e.Message);
        }
    }

    private void SetupRequiredPaths()
    {
        try
        {
            WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Wolven Icon Generator", "working");
            WolvenKitTempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            ImageImportDirectory = Path.Combine(WorkingDirectory, "imported");

            Directory.CreateDirectory(WorkingDirectory);
            Directory.CreateDirectory(WolvenKitTempDirectory);
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("SetupPaths").Error(e.Message);
        }
    }

    #region Creating a faux project

    /// <summary>
    /// Create the necessary directories for the import operation based on the station ID and atlas name. Optionally, overwrite the existing directories.
    /// </summary>
    /// <param name="atlasName">The name that icon atlas will be generated with.</param>
    /// <param name="overwrite">Indicates whether existing directories should be overwritten if they exist.</param>
    /// <returns>A dictionary where the key is the type of project folder and the value is the folder path.
    /// <list type="bullet">
    ///     <item>"importedPngs" - input to the generate ink atlas json function: <c>tools\\imported\\{stationID}\\atlasName</c>.</item>
    ///     <item>"iconFiles" - output folder for the converted .inkatlas and .xbm files: <c>tools\\atlasName\\archive\\base\\icon</c>.</item>
    ///     <item>"projectBasePath" - the faux project's base path: <c>tools\\atlasName</c></item>
    ///     <item>"projectRawPath" - the faux project's raw path: <c>tools\\atlasName\\source\\raw</c>.</item>
    ///     <item>"archiveBasePath" - the base path to the archive folder within the project: <c>tools\\atlasName\\archive</c></item>
    /// </list>
    /// If any of the directories could not be created or an error occurred, an empty dictionary is returned.
    /// </returns>
    private Dictionary<string, string> CreateImportDirectories(string atlasName, bool overwrite = false)
    {
        try
        {
            if (WorkingDirectory == null)
                throw new InvalidOperationException("The working directory is null.");

            if (ImageImportDirectory == null)
                throw new InvalidOperationException("The image import directory is null.");

            var outputDictionary = new Dictionary<string, string>();

            //Create the path that imported PNGs are stored: %APPDATA%\Wolven Icon Generator\tools\imported\{atlasName-Guid}
            var importedPngsPath = Path.Combine(ImageImportDirectory, $"{atlasName}-{Guid.NewGuid().ToString()}");
            if (overwrite && Directory.Exists(importedPngsPath))
                Directory.Delete(importedPngsPath, true);
            Directory.CreateDirectory(importedPngsPath);
            outputDictionary["importedPngs"] = importedPngsPath;

            //Create the project's base path; also the folder for the final .archive file to be stored.
            var projectBasePath = Path.Combine(WorkingDirectory, "tools", atlasName);
            if (overwrite && Directory.Exists(projectBasePath))
                Directory.Delete(projectBasePath, true);
            Directory.CreateDirectory(projectBasePath);
            outputDictionary["projectBasePath"] = projectBasePath;

            //Create the path for the .inkatlas and .xbm files to be stored in.
            var iconFilesPath = Path.Combine(projectBasePath, "archive", "base", "icon");
            if (overwrite && Directory.Exists(iconFilesPath))
                Directory.Delete(iconFilesPath, true);
            Directory.CreateDirectory(iconFilesPath);
            outputDictionary["iconFiles"] = iconFilesPath;
            outputDictionary["archiveBasePath"] = Path.Combine(projectBasePath, "archive");

            //Create the path that the import project files will be stored: %APPDATA%\Wolven Icon Generator\tools\{atlasName}\source\raw
            var projectRawPath = Path.Combine(projectBasePath, "source", "raw");
            if (overwrite && Directory.Exists(projectRawPath))
                Directory.Delete(projectRawPath, true);
            Directory.CreateDirectory(projectRawPath);
            outputDictionary["projectRawPath"] = projectRawPath;

            return outputDictionary;
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("CreateImportDirectories").Error(e.Message);
        }

        return [];
    }

    #endregion

    #region WolvenKit Download and Extraction

    /// <summary>
    /// Downloads the WolvenKit CLI if it has not been downloaded yet and extracts it to the temporary directory: <see cref="WolvenKitTempDirectory"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Occurs if either <see cref="WorkingDirectory"/> or <see cref="WolvenKitTempDirectory"/> are <c>null</c>.</exception>
    private async Task DownloadWolvenKitIfRequiredAsync()
    {
        if (_isWolvenKitDownloaded && _isWolvenKitExtracted) return;

        try
        {
            if (WorkingDirectory == null || WolvenKitTempDirectory == null)
                throw new InvalidOperationException("Working directory or WolvenKit temp directory is null.");

            var zipFile = Path.Combine(WorkingDirectory, $"WolvenKit.Console-{WolvenKitVersion}.zip");

            if (File.Exists(zipFile))
            {
                _isWolvenKitDownloaded = true;
                if (!await ExtractWolvenKitAsync(zipFile))
                    throw new InvalidOperationException("The WolvenKit CLI could not be extracted.");
            }
            else
            {
                if (!await DownloadWolvenKitAsync(zipFile))
                    throw new InvalidOperationException("The WolvenKit CLI could not be downloaded.");

                if (!await ExtractWolvenKitAsync(zipFile))
                    throw new InvalidOperationException("The WolvenKit CLI could not be extracted.");
            }
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("DownloadWolvenKitIfRequired").Error(e.Message);
        }
    }

    /// <summary>
    /// Asynchronously downloads the WolvenKit CLI from the specified URL.
    /// </summary>
    /// <param name="zipFile">The zip file to save.</param>
    /// <returns>A task representing the asynchronous download operation.</returns>
    private async Task<bool> DownloadWolvenKitAsync(string zipFile)
    {
        if (_wolvenKitCliDownloadUrl == null)
            throw new InvalidOperationException("The WolvenKit CLI download URL is null.");

        await PathHelper.DownloadFileAsync(_wolvenKitCliDownloadUrl, zipFile);
        if (!File.Exists(zipFile)) return _isWolvenKitDownloaded;

        _isWolvenKitDownloaded = true;
        AuLogger.GetCurrentLogger<IconManager>().Info("WolvenKit CLI downloaded successfully.");
        return _isWolvenKitDownloaded;
    }

    /// <summary>
    /// Asynchronously extracts the WolvenKit CLI from the specified zip file.
    /// </summary>
    /// <param name="zipFile">The zip file to extract.</param>
    /// <returns>A task representing the asynchronous extraction operation.</returns>
    private async Task<bool> ExtractWolvenKitAsync(string zipFile)
    {
        if (WolvenKitTempDirectory == null)
            throw new InvalidOperationException("The WolvenKit temp directory is null.");

        await PathHelper.ExtractZipFileAsync(zipFile, WolvenKitTempDirectory);
        if (File.Exists(_wolvenKitCliExe))
        {
            _isWolvenKitExtracted = true;
            AuLogger.GetCurrentLogger<IconManager>().Info("WolvenKit CLI extracted successfully.");
        }
        return _isWolvenKitExtracted;
    }

    #endregion

    #region Import - Creating .archive files

    private int _currentProgress;

    /// <summary>
    /// Import a PNG image file as a custom icon for a station.
    /// </summary>
    /// <param name="imagePath">The path to the image file on disk.</param>
    /// <param name="atlasName">The name that icon atlas will be generated with.</param>
    /// <param name="outputPath">The path that the final <c>.archive</c> file should be saved to.</param>
    /// <param name="overwrite">Indicates whether existing directories should be overwritten if they exist.</param>
    /// <returns>A task that when complete contains an <see cref="WolvenIcon"/> or <c>null</c> if the import failed.</returns>
    public async Task<WolvenIcon?> ImportIconImageAsync(string imagePath, string atlasName, string outputPath, bool overwrite = true)
    {
        if (!IsInitialized)
            throw new InvalidOperationException("The icon manager has not been initialized.");

        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        try
        {
            OnIconImportStarted(new StatusEventArgs("The icon import operation has started.", false, _currentProgress));
            WolvenIcon? icon = null;

            await Task.Run(async () =>
            {
                icon = await CreateIcon(imagePath, atlasName, outputPath, overwrite, token);
                if (icon == null)
                    throw new InvalidOperationException("The icon could not be created.");
            }, token);
            return icon;
        }
        catch (OperationCanceledException)
        {
            AuLogger.GetCurrentLogger<IconManager>("ImportIconImageAsync").Info("The icon import operation was cancelled.");
            OnIconImportStatus(new StatusEventArgs("The icon import operation was cancelled.", false, _currentProgress));
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("ImportIconAsync").Error(e.Message);
            OnIconImportStatus(new StatusEventArgs(e.Message, true, _currentProgress));
        }
        finally
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            ResetProgress();
        }

        return null;
    }

    private async Task<WolvenIcon> CreateIcon(string imagePath, string atlasName, string outputPath, bool overwrite, CancellationToken token)
    {
        if (!IsInitialized)
        {
            OnIconImportStatus(new StatusEventArgs("The icon manager has not been initialized.", true, _currentProgress));
            CancelOperation();
        }
        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        if (!File.Exists(imagePath))
        {
            OnIconImportStatus(new StatusEventArgs("The image file could not be found.", true, _currentProgress));
            CancelOperation();
        }
        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        if (!ImageUtils.IsPngFile(imagePath))
        {
            OnIconImportStatus(new StatusEventArgs("The image file is not a PNG file.", true, _currentProgress));
            CancelOperation();
        }
        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        var projectDirectories = CreateImportDirectories(atlasName, overwrite);
        if (projectDirectories.Count == 0)
        {
            OnIconImportStatus(new StatusEventArgs("The project directories could not be created.", true, _currentProgress));
            CancelOperation();
        }
        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        //Copy the image to the images folder for the project
        var imageFileName = Path.GetFileName(imagePath);
        var projectImagePath = Path.Combine(projectDirectories["importedPngs"], imageFileName);
        File.Copy(imagePath, projectImagePath, overwrite);

        if (_inkAtlasExe.Equals(string.Empty) || _inkAtlasCli == null)
        {
            OnIconImportStatus(new StatusEventArgs("The inkatlas executable could not be found.", true, _currentProgress));
            CancelOperation();
        }
        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        //Generate .archive file
        await _inkAtlasCli.GenerateInkAtlasJsonAsync(projectDirectories["importedPngs"], projectDirectories["projectRawPath"], atlasName);

        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        await _wolvenKitCli.ConvertToInkAtlasFile(projectDirectories["projectRawPath"]);

        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        await _wolvenKitCli.ImportToWolvenKitProject(projectDirectories["projectRawPath"]);

        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        //Copy files needed to the icon project folder
        CopyProjectFiles(projectDirectories["projectRawPath"], projectDirectories["iconFiles"]);

        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        //Pack the icon project folder into a .archive file
        await _wolvenKitCli.PackArchive(projectDirectories["archiveBasePath"], projectDirectories["projectBasePath"]);

        //Rename the .archive file to the atlas name
        var originalArchivePath = Path.Combine(projectDirectories["projectBasePath"], "archive.archive");
        var newArchivePath = Path.Combine(projectDirectories["projectBasePath"], $"{atlasName}.archive");
        File.Copy(originalArchivePath, newArchivePath, overwrite);
        File.Delete(originalArchivePath);

        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        //Copy the archive to the staging icons folder
        var finalOutputPath = Path.Combine(outputPath, $"{atlasName}.archive");
        File.Copy(newArchivePath, finalOutputPath, overwrite);

        //Finally, create the icon object and return it
        var icon = new WolvenIcon(projectImagePath, finalOutputPath, atlasName)
        {
            CustomIcon = new RadioExtCustomIcon()
            {
                InkAtlasPath = Path.Combine("base", "icon", $"{atlasName}.inkatlas"),
                InkAtlasPart = $"{atlasName}"
            }
        };

        OnIconImportFinished(new StatusEventArgs("The icon import operation has completed successfully.", false, _currentProgress));

        return icon;
    }

    /// <summary>
    /// Copy the required .inkatlas and .xbm files to the project's icon folder. This is the folder that will be packed into a .archive file.
    /// </summary>
    /// <param name="sourcePath">The path to the project's raw files.</param>
    /// <param name="destinationPath">The project's icon folder.</param>
    private void CopyProjectFiles(string sourcePath, string destinationPath)
    {
        try
        {
            if (!Directory.Exists(sourcePath))
                throw new DirectoryNotFoundException("The source path does not exist.");

            if (!Directory.Exists(destinationPath))
                throw new DirectoryNotFoundException("The destination path does not exist.");

            foreach (var file in Directory.GetFiles(sourcePath))
            {
                var fileName = Path.GetFileName(file);
                if (!fileName.EndsWith(".inkatlas") && !fileName.EndsWith(".xbm")) continue;

                var destFile = Path.Combine(destinationPath, fileName);
                File.Copy(file, destFile, true);
            }
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("CopyProjectFiles").Error(e.Message);
        }
    }

    private void InkAtlasCli_OutputChanged(object? sender, string? e)
    {
        _currentProgress += 2;
        if (e != null)
            OnIconImportStatus(new StatusEventArgs(e, false, _currentProgress));
    }

    private void InkAtlasCli_ErrorChanged(object? sender, string? e)
    {
        if (e != null)
            OnIconImportStatus(new StatusEventArgs(e, true, _currentProgress));
    }

    private void WolvenKitCli_OutputChanged(object? sender, string? e)
    {
        _currentProgress += 2;
        if (e != null)
            OnIconExportStatus(new StatusEventArgs(e, false, _currentProgress));
    }

    private void WolvenKitCli_ErrorChanged(object? sender, string? e)
    {
        if (e != null)
            OnIconExportStatus(new StatusEventArgs(e, true, _currentProgress));
    }

    /// <summary>
    /// Cancel the currently running operation.
    /// </summary>
    public void CancelOperation() => _cancellationTokenSource?.Cancel();

    #endregion

    #region Helper Methods
    /// <summary>
    /// Reset the current progress percentage to 0.
    /// </summary>
    private void ResetProgress() => _currentProgress = 0;
    #endregion

    #region Event Handlers
    private void OnIconImportStarted(StatusEventArgs e) => IconImportStarted?.Invoke(this, e);
    private void OnIconImportStatus(StatusEventArgs e) => CliStatus?.Invoke(this, e);
    private void OnIconImportFinished(StatusEventArgs e) => IconImportFinished?.Invoke(this, e);
    private void OnIconExportStarted(StatusEventArgs e) => IconExportStarted?.Invoke(this, e);
    private void OnIconExportStatus(StatusEventArgs e) => CliStatus?.Invoke(this, e);
    private void OnIconExportFinished(StatusEventArgs e) => IconExportFinished?.Invoke(this, e);
    #endregion
}