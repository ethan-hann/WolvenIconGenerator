using AetherUtils.Core.Files;
using AetherUtils.Core.Logging;
using System.Drawing;
using WIG.Lib.Models;
using WIG.Lib.Tools.InkAtlas;

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
    private string _wolvenKitCliExe = string.Empty;
    private bool _isWolvenKitDownloaded;
    private bool _isWolvenKitExtracted;
    private CancellationTokenSource? _cancellationTokenSource;

    private readonly InkAtlasGenerator? _inkAtlasGenerator;
    private readonly Json<InkAtlasData> _inkAtlasSerializer = new();
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

    public string? ImageExportDirectory { get; private set; }

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

            _inkAtlasGenerator = new InkAtlasGenerator();
            _inkAtlasGenerator.OutputChanged += InkAtlasGenerator_OutputChanged;
            _inkAtlasGenerator.ErrorChanged += InkAtlasGenerator_ErrorChanged;
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
            //_inkAtlasExe = Assembly.GetExecutingAssembly().ExtractEmbeddedResource("WIG.Lib.Tools.InkAtlas.generate_inkatlas.exe");

            if (WolvenKitTempDirectory == null)
                throw new InvalidOperationException("The WolvenKit temp directory is null.");

            _wolvenKitCliExe = Path.Combine(WolvenKitTempDirectory, "WolvenKit.CLI.exe");

            await DownloadWolvenKitIfRequiredAsync();

            if (!File.Exists(_wolvenKitCliExe))
                throw new FileNotFoundException("The WolvenKit CLI executable could not be found.", _wolvenKitCliExe);

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
            ImageExportDirectory = Path.Combine(WorkingDirectory, "exported");

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
    ///     <item>"importedPngs" - the path that imported PNGs are stored: %APPDATA%\Wolven Icon Generator\tools\imported\{atlasName-iconId}.</item>
    ///     <item>"projectBasePath" - the base path for the project: %APPDATA%\Wolven Icon Generator\tools\{atlasName}.</item>
    ///     <item>"archiveBasePath" - the base path for the REDEngine files: %APPDATA%\Wolven Icon Generator\tools\{atlasName}\source\archive.</item>
    ///     <item>"redEngineFilesPath" - the path to the REDEngine files: %APPDATA%\Wolven Icon Generator\tools\{atlasName}\source\archive\base\icon.</item>
    ///     <item>"rawFilesBasePath" - the base path for the raw files: %APPDATA%\Wolven Icon Generator\tools\{atlasName}\source\raw.</item>
    ///     <item>"rawFilesPath" - the path to the raw (non-REDEngine) files: %APPDATA%\Wolven Icon Generator\tools\{atlasName}\source\raw\base\icon.</item>
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

            //Create the path that imported PNGs are stored: %APPDATA%\Wolven Icon Generator\tools\imported\{atlasName-iconId}
            var importedPngsPath = Path.Combine(ImageImportDirectory, $"{atlasName}-{Guid.NewGuid()}");
            if (overwrite && Directory.Exists(importedPngsPath))
                Directory.Delete(importedPngsPath, true);
            Directory.CreateDirectory(importedPngsPath);
            outputDictionary["importedPngs"] = importedPngsPath;

            //Create the base path for the project: %APPDATA%\Wolven Icon Generator\tools\{atlasName}
            var projectBasePath = Path.Combine(WorkingDirectory, "tools", atlasName);
            if (overwrite && Directory.Exists(projectBasePath))
                Directory.Delete(projectBasePath, true);
            Directory.CreateDirectory(projectBasePath);
            outputDictionary["projectBasePath"] = projectBasePath;

            //Create the base path for the REDEngine files: %APPDATA%\Wolven Icon Generator\tools\{atlasName}\source\archive
            var archiveBasePath = Path.Combine(projectBasePath, "source", "archive");
            if (overwrite && Directory.Exists(archiveBasePath))
                Directory.Delete(archiveBasePath, true);
            Directory.CreateDirectory(archiveBasePath);
            outputDictionary["archiveBasePath"] = archiveBasePath;

            //Create the path to the REDEngine files: %APPDATA%\Wolven Icon Generator\tools\{atlasName}\source\archive\base\icon
            var redEngineFilesPath = Path.Combine(archiveBasePath, "base", "icon");
            if (overwrite && Directory.Exists(redEngineFilesPath))
                Directory.Delete(redEngineFilesPath, true);
            Directory.CreateDirectory(redEngineFilesPath);
            outputDictionary["redEngineFilesPath"] = redEngineFilesPath;

            //Create the base path for the raw files: %APPDATA%\Wolven Icon Generator\tools\{atlasName}\source\raw
            var rawFilesBasePath = Path.Combine(projectBasePath, "source", "raw");
            if (overwrite && Directory.Exists(rawFilesBasePath))
                Directory.Delete(rawFilesBasePath, true);
            Directory.CreateDirectory(rawFilesBasePath);
            outputDictionary["rawFilesBasePath"] = rawFilesBasePath;

            //Create the path to the raw (non-REDEngine) files: %APPDATA%\Wolven Icon Generator\tools\{atlasName}\source\raw\base\icon
            var rawFilesPath = Path.Combine(rawFilesBasePath, "base", "icon");
            if (overwrite && Directory.Exists(rawFilesPath))
                Directory.Delete(rawFilesPath, true);
            Directory.CreateDirectory(rawFilesPath);
            outputDictionary["rawFilesPath"] = rawFilesPath;

            return outputDictionary;
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("CreateImportDirectories").Error(e, e.Message);
        }

        return [];
    }

    private Dictionary<string, string> CreateExportDirectories(string atlasName, bool overwrite = false)
    {
        try
        {
            if (WorkingDirectory == null)
                throw new InvalidOperationException("The working directory is null.");

            if (ImageExportDirectory == null)
                throw new InvalidOperationException("The image export directory is null.");

            var outputDictionary = new Dictionary<string, string>();

            //Create the path that imported PNGs are stored: %APPDATA%\Wolven Icon Generator\tools\exported\{atlasName-iconId}
            var exportedPngsPath = Path.Combine(ImageExportDirectory, $"{atlasName}-{Guid.NewGuid()}");
            if (overwrite && Directory.Exists(exportedPngsPath))
                Directory.Delete(exportedPngsPath, true);
            Directory.CreateDirectory(exportedPngsPath);
            outputDictionary["exportedPngs"] = exportedPngsPath;

            //Create the base path for the project: %APPDATA%\Wolven Icon Generator\tools\{atlasName}
            var projectBasePath = Path.Combine(WorkingDirectory, "tools", atlasName);
            if (overwrite && Directory.Exists(projectBasePath))
                Directory.Delete(projectBasePath, true);
            Directory.CreateDirectory(projectBasePath);
            outputDictionary["projectBasePath"] = projectBasePath;

            ////Create the base path for the REDEngine files: %APPDATA%\Wolven Icon Generator\tools\{atlasName}\source\archive
            //var archiveBasePath = Path.Combine(projectBasePath, "source", "archive");
            //if (overwrite && Directory.Exists(archiveBasePath))
            //    Directory.Delete(archiveBasePath, true);
            //Directory.CreateDirectory(archiveBasePath);
            //outputDictionary["archiveBasePath"] = archiveBasePath;

            ////Create the path to the REDEngine files: %APPDATA%\Wolven Icon Generator\tools\{atlasName}\source\archive\base\icon
            //var redEngineFilesPath = Path.Combine(archiveBasePath, "base", "icon");
            //if (overwrite && Directory.Exists(redEngineFilesPath))
            //    Directory.Delete(redEngineFilesPath, true);
            //Directory.CreateDirectory(redEngineFilesPath);
            //outputDictionary["redEngineFilesPath"] = redEngineFilesPath;

            ////Create the base path for the raw files: %APPDATA%\Wolven Icon Generator\tools\{atlasName}\source\raw
            //var rawFilesBasePath = Path.Combine(projectBasePath, "source", "raw");
            //if (overwrite && Directory.Exists(rawFilesBasePath))
            //    Directory.Delete(rawFilesBasePath, true);
            //Directory.CreateDirectory(rawFilesBasePath);
            //outputDictionary["rawFilesBasePath"] = rawFilesBasePath;

            ////Create the path to the raw (non-REDEngine) files: %APPDATA%\Wolven Icon Generator\tools\{atlasName}\source\raw\base\icon
            //var rawFilesPath = Path.Combine(rawFilesBasePath, "base", "icon");
            //if (overwrite && Directory.Exists(rawFilesPath))
            //    Directory.Delete(rawFilesPath, true);
            //Directory.CreateDirectory(rawFilesPath);
            //outputDictionary["rawFilesPath"] = rawFilesPath;

            return outputDictionary;
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("CreateExportDirectories").Error(e, e.Message);
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
    /// Generate a <c>.archive</c> file from a PNG image.
    /// This method will create a faux project, generate the necessary files, and pack them into a <c>.archive</c> file.
    /// <para>The resulting <c>.archive</c> file path can then be referenced using the <see cref="WolvenIcon"/> object created: <see cref="WolvenIcon.ArchivePath"/></para>
    /// </summary>
    /// <param name="imagePath">The path to the image file on disk.</param>
    /// <param name="atlasName">The name that icon atlas will be generated with.</param>
    /// <param name="overwrite">Indicates whether existing directories should be overwritten if they exist.</param>
    /// <returns>A task that when complete contains an <see cref="WolvenIcon"/> or <c>null</c> if the import failed.</returns>
    /// <exception cref="InvalidOperationException">Occurs when the icon manager has not been initialized.</exception>
    public async Task<WolvenIcon?> GenerateIconImageAsync(string imagePath, string atlasName, bool overwrite = true)
    {
        _currentProgress = 0;
        if (!IsInitialized)
            throw new InvalidOperationException("The icon manager has not been initialized.");

        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        try
        {
            AuLogger.GetCurrentLogger<IconManager>("GenerateIconImageAsync").Info("The icon import operation has started.");
            OnIconImportStarted(new StatusEventArgs("The icon import operation has started.", false, _currentProgress));
            WolvenIcon? icon = null;

            await Task.Run(async () =>
            {
                icon = await CreateIcon(imagePath, atlasName, overwrite, token);
                if (icon == null)
                    throw new InvalidOperationException("The icon could not be created.");
            }, token);

            AuLogger.GetCurrentLogger<IconManager>("GenerateIconImageAsync").Info("The icon import operation has completed successfully.");
            OnIconImportFinished(new StatusEventArgs("The icon import operation has completed successfully.", false, _currentProgress));
            return icon;
        }
        catch (OperationCanceledException)
        {
            AuLogger.GetCurrentLogger<IconManager>("GenerateIconImageAsync").Info("The icon import operation was cancelled.");
            OnIconImportStatus(new StatusEventArgs("The icon import operation was cancelled.", false, _currentProgress));
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("GenerateIconImageAsync").Error(e, e.Message);
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

    /// <summary>
    /// Create a WolvenIcon object from the specified image path and atlas name.
    /// This method will create the necessary directories, copy the image to the project folder, generate the .inkatlas.json and .archive files,
    /// and pack the project folder into a .archive file.
    /// </summary>
    /// <param name="imagePath">The path to the PNG file to create the icon of.</param>
    /// <param name="atlasName">The name that the atlas should be created with. Must be all lowercase.</param>
    /// <param name="overwrite">Indicates whether existing files should be overwritten.</param>
    /// <param name="token">The token used for cancelling running operations.</param>
    /// <returns>A task, that when complete, contains the resulting <see cref="WolvenIcon"/> object.</returns>
    private async Task<WolvenIcon> CreateIcon(string imagePath, string atlasName, bool overwrite, CancellationToken token)
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

        // Copy the image to the images folder for the project
        var imageFileName = Path.GetFileName(imagePath);
        var projectImagePath = Path.Combine(projectDirectories["importedPngs"], imageFileName);
        File.Copy(imagePath, projectImagePath, overwrite);

        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        // Use InkAtlasGenerator to generate the .inkatlas.json and images
        await _inkAtlasGenerator.GenerateInkAtlasJsonAsync(projectDirectories["importedPngs"], projectDirectories["rawFilesPath"], atlasName);

        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        await _wolvenKitCli.ConvertToInkAtlasFile(projectDirectories["rawFilesPath"]);

        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        await _wolvenKitCli.ImportToWolvenKitProject(projectDirectories["rawFilesPath"]);

        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        //Copy files needed to the icon project folder
        CopyProjectFiles(projectDirectories["rawFilesPath"], projectDirectories["redEngineFilesPath"]);

        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        //Pack the icon project folder into a .archive file
        await _wolvenKitCli.PackArchive(projectDirectories["archiveBasePath"], projectDirectories["projectBasePath"]);

        //Rename the .archive file to the atlas name
        var originalArchivePath = Path.Combine(projectDirectories["projectBasePath"], "archive.archive");
        var newArchivePath = Path.Combine(projectDirectories["projectBasePath"], $"{atlasName}.archive");
        File.Copy(originalArchivePath, newArchivePath, overwrite);

        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        //Copy the archive to the staging icons folder
        if (!Path.Exists(newArchivePath))
        {
            OnIconImportStatus(new StatusEventArgs("The final .archive file could not be renamed.", true, _currentProgress));
            CancelOperation();
        }

        File.Delete(originalArchivePath);

        //Finally, create the icon object and return it
        var icon = new WolvenIcon(projectImagePath)
        {
            CustomIcon =
            {
                InkAtlasPath = Path.Combine("base", "icon", $"{atlasName}.inkatlas"),
                InkAtlasPart = "icon_part"
            },
            AtlasName = atlasName,
            OriginalArchivePath = newArchivePath,
            Sha256HashOfArchiveFile = HashUtils.ComputeSha256Hash(newArchivePath, true)
        };

        return icon;
    }

    /// <summary>
    /// Copy the required .inkatlas and .xbm files to the project's REDEngine folder and remove them from the raw files folder.
    /// </summary>
    /// <param name="rawFilesPath">The path to the project's raw files.</param>
    /// <param name="redEngineFilesPath">The project's icon folder.</param>
    private void CopyProjectFiles(string rawFilesPath, string redEngineFilesPath)
    {
        try
        {
            if (!Directory.Exists(rawFilesPath))
                throw new DirectoryNotFoundException($"The path does not exist: {rawFilesPath}");

            if (!Directory.Exists(redEngineFilesPath))
                throw new DirectoryNotFoundException($"The path does not exist: {redEngineFilesPath}");

            List<string> filesToDelete = [];
            foreach (var file in Directory.GetFiles(rawFilesPath))
            {
                var fileName = Path.GetFileName(file);
                if (!fileName.EndsWith(".inkatlas") && !fileName.EndsWith(".xbm")) continue;

                var destFile = Path.Combine(redEngineFilesPath, fileName);
                File.Copy(file, destFile, true);
                filesToDelete.Add(file);
            }

            filesToDelete.ForEach(File.Delete);
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("CopyProjectFiles").Error(e.Message);
        }
    }

    #endregion

    #region Export - Extracting .archive files

    /// <summary>
    /// Extract (unbundle) the icon from the specified .archive file.
    /// This method will create a faux project, unpack the .archive file, convert the XBM files to PNGs, and create the WolvenIcon object.
    /// </summary>
    /// <param name="archivePath">The path to a .archive file to unbundle.</param>
    /// <param name="overwrite">Indicates whether existing directories should be overwritten if they exist.</param>
    /// <returns>A task that when complete contains an <see cref="WolvenIcon"/> or <c>null</c> if the extract failed.</returns>
    /// <exception cref="InvalidOperationException">Occurs when the icon manager has not been initialized.</exception>
    public async Task<WolvenIcon?> ExtractIconImageAsync(string archivePath, bool overwrite = true)
    {
        _currentProgress = 0;
        if (!IsInitialized)
            throw new InvalidOperationException("The icon manager has not been initialized.");

        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        try
        {
            AuLogger.GetCurrentLogger<IconManager>("ExtractIconImageAsync").Info("The icon export operation has started.");
            OnIconExportStarted(new StatusEventArgs("The icon export operation has started.", false, _currentProgress));
            WolvenIcon? icon = null;

            await Task.Run(async () =>
            {
                icon = await ExtractIcon(archivePath, overwrite, token);
                if (icon == null)
                    throw new InvalidOperationException("The icon could not be created.");
            }, token);

            AuLogger.GetCurrentLogger<IconManager>("ExtractIconImageAsync").Info("The icon export operation has completed successfully.");
            OnIconExportFinished(new StatusEventArgs("The icon export operation has completed successfully.", false, _currentProgress));
            return icon;
        }
        catch (OperationCanceledException)
        {
            AuLogger.GetCurrentLogger<IconManager>("ExtractIconImageAsync").Info("The icon export operation was cancelled.");
            OnIconExportStatus(new StatusEventArgs("The icon export operation was cancelled.", false, _currentProgress));
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("ExtractIconImageAsync").Error(e, e.Message);
            OnIconExportStatus(new StatusEventArgs(e.Message, true, _currentProgress));
        }
        finally
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            ResetProgress();
        }

        return null;
    }

    /// <summary>
    /// Create a WolvenIcon object from the specified .archive file. 
    /// This method will extract the .archive file, convert the XBM files to PNGs, and create the WolvenIcon object.
    /// It will read the .inkatlas.json file to determine the InkAtlas path and part.
    /// </summary>
    /// <param name="archivePath">The path to a .archive file to unbundle.</param>
    /// <param name="overwrite">Indicates whether existing directories should be overwritten if they exist.</param>
    /// <param name="token">The token used for cancelling running operations.</param>
    /// <returns>A task, that when complete, contains the resulting <see cref="WolvenIcon"/> object.</returns>
    private async Task<WolvenIcon> ExtractIcon(string archivePath, bool overwrite, CancellationToken token)
    {
        if (!IsInitialized)
        {
            OnIconExportStatus(new StatusEventArgs("The icon manager has not been initialized.", true, _currentProgress));
            CancelOperation();
        }
        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        if (!File.Exists(archivePath))
        {
            OnIconExportStatus(new StatusEventArgs("The archive file could not be found.", true, _currentProgress));
            CancelOperation();
        }
        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        var atlasName = Path.GetFileNameWithoutExtension(archivePath);

        var projectDirectories = CreateExportDirectories(atlasName, overwrite);
        if (projectDirectories.Count == 0)
        {
            OnIconExportStatus(new StatusEventArgs("The project directories could not be created.", true, _currentProgress));
            CancelOperation();
        }
        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        // Unpack the archive file
        await _wolvenKitCli.UnpackArchive(archivePath, projectDirectories["projectBasePath"]);

        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        //Convert the XBM files to PNG
        var xbmFiles = Directory.GetFiles(projectDirectories["projectBasePath"], "*.xbm", SearchOption.AllDirectories);
        if (xbmFiles.Length == 0)
        {
            OnIconExportStatus(new StatusEventArgs("No XBM files were found in the project folder.", true, _currentProgress));
            CancelOperation();
        }
        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        foreach (var xbmFile in xbmFiles)
        {
            await _wolvenKitCli.ExportPng(xbmFile, projectDirectories["exportedPngs"]);
            _currentProgress += 2;
            token.ThrowIfCancellationRequested();
        }

        //Get the PNG files from the exported folder
        var pngFiles = Directory.GetFiles(projectDirectories["exportedPngs"], "*.png");

        //Rename the .archive file to the atlas name
        var newArchivePath = Path.Combine(projectDirectories["projectBasePath"], $"{Path.GetFileNameWithoutExtension(archivePath)}.archive");
        File.Copy(archivePath, newArchivePath, overwrite);

        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        //Copy the archive to the staging icons folder
        if (!Path.Exists(newArchivePath))
        {
            OnIconExportStatus(new StatusEventArgs("The final .archive file could not be copied.", true, _currentProgress));
            CancelOperation();
        }

        //Make sure we have at least one PNG
        if (pngFiles.Length == 0)
        {
            OnIconExportStatus(new StatusEventArgs("No PNG files were found in the exported folder.", true, _currentProgress));
            CancelOperation();
        }

        //Get the InkAtlas path and part
        var pathAndParts = await GetInkAtlasPathAndPart(projectDirectories["projectBasePath"], xbmFiles.First());

        _currentProgress += 2;
        token.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(pathAndParts.inkAtlasPath) || string.IsNullOrEmpty(pathAndParts.inkAtlasPart))
        {
            OnIconExportStatus(new StatusEventArgs("The .inkatlas path and part could not be determined.", true, _currentProgress));
            CancelOperation();
        }

        //Finally, create the icon object from the first PNG file and return it
        var icon = new WolvenIcon(pngFiles.First(), newArchivePath)
        {
            CustomIcon =
            {
                InkAtlasPath = pathAndParts.inkAtlasPath,
                InkAtlasPart = pathAndParts.inkAtlasPart
            },
            AtlasName = atlasName,
            OriginalArchivePath = archivePath,
        };
        return icon;
    }

    private async Task<(string inkAtlasPath, string inkAtlasPart)> GetInkAtlasPathAndPart(string projectBasePath, string xbmFile)
    {
        try
        {
            if (!Directory.Exists(projectBasePath))
                throw new DirectoryNotFoundException($"The path does not exist: {projectBasePath}");

            var inkAtlasFile = Directory.GetFiles(projectBasePath, "*.inkatlas", SearchOption.AllDirectories).FirstOrDefault();

            if (inkAtlasFile == null)
                throw new FileNotFoundException("The .inkatlas file could not be found.", projectBasePath);

            var inkAtlasPath = PathHelper.GetRelativePath(projectBasePath, inkAtlasFile);

            if (inkAtlasPath == null)
                throw new InvalidOperationException("The relative path could not be determined.");

            await _wolvenKitCli.ConvertToInkAtlasJsonFile(inkAtlasFile);

            var inkAtlasJsonFile = Directory.GetFiles(projectBasePath, "*.json", SearchOption.AllDirectories).FirstOrDefault();
            if (inkAtlasJsonFile == null)
                throw new FileNotFoundException("The .inkatlas.json file could not be found.", projectBasePath);

            var atlasData = _inkAtlasSerializer.LoadJson(inkAtlasJsonFile);
            if (atlasData == null)
                throw new InvalidOperationException("The .inkatlas.json file could not be loaded.");

            var expectedPathInJson = PathHelper.GetRelativePath(projectBasePath, xbmFile);

            //The part name is the first part which has a texture with the expected path
            var partName = atlasData.Data.RootChunk.Slots.Elements.First(e => e.Texture.DepotPath.Value.Equals(expectedPathInJson)).Parts.First().PartName.Value;

            //The path to the .inkatlas file should match the expected path of the .xbm file in the .inkatlas.json file but with the .inkatlas file name
            inkAtlasPath = Path.Combine(expectedPathInJson[..expectedPathInJson.LastIndexOf('\\')],
                Path.GetFileName(inkAtlasFile));
            return (inkAtlasPath, partName);
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("GetInkAtlasPathAndPart").Error(e, e.Message);
        }

        return (string.Empty, string.Empty);
    }

    #endregion

    #region Helper Methods
    /// <summary>
    /// Reset the current progress percentage to 0.
    /// </summary>
    private void ResetProgress() => _currentProgress = 0;

    /// <summary>
    /// Cancel the currently running operation.
    /// </summary>
    public void CancelOperation() => _cancellationTokenSource?.Cancel();
    #endregion

    #region Event Handlers
    private void OnIconImportStarted(StatusEventArgs e) => IconImportStarted?.Invoke(this, e);
    private void OnIconImportStatus(StatusEventArgs e) => CliStatus?.Invoke(this, e);
    private void OnIconImportFinished(StatusEventArgs e) => IconImportFinished?.Invoke(this, e);
    private void OnIconExportStarted(StatusEventArgs e) => IconExportStarted?.Invoke(this, e);
    private void OnIconExportStatus(StatusEventArgs e) => CliStatus?.Invoke(this, e);
    private void OnIconExportFinished(StatusEventArgs e) => IconExportFinished?.Invoke(this, e);

    private void InkAtlasGenerator_OutputChanged(object? sender, string? e)
    {
        _currentProgress += 2;
        if (e != null)
            OnIconImportStatus(new StatusEventArgs(e, false, _currentProgress));
    }

    private void InkAtlasGenerator_ErrorChanged(object? sender, string? e)
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
    #endregion
}