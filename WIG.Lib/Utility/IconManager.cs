using AetherUtils.Core.Files;
using AetherUtils.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIG.Lib.Models;
using WIG.Lib.Tools.InkAtlas;

namespace WIG.Lib.Utility
{
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

        private int _currentProgress;

        /// <summary>
        /// Gets the singleton instance of the <see cref="IconManager"/>.
        /// </summary>
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
        /// Gets or sets the current version of WolvenKit to download. Defaults to <c>8.14.0</c>.
        /// </summary>
        public string? WolvenKitVersion { get; set; } = "8.14.0";

        /// <summary>
        /// Gets the working directory for the icon generator. Defaults to <c>%LOCALAPPDATA%\RadioExt-Helper\tools</c>.
        /// </summary>
        public string? WorkingDirectory { get; private set; }

        /// <summary>
        /// Gets the temporary directory for the icon generator. Defaults to <c>%TEMP%\{random folder name}</c>.
        /// </summary>
        public string? WolvenKitTempDirectory { get; private set; }

        /// <summary>
        /// Gets the path to the image import directory. Defaults to <c>%LOCALAPPDATA%\RadioExt-Helper\tools\imported</c>.
        /// <para>This directory is where .png files that have been imported are stored. The file names are GUIDs.</para>
        /// </summary>
        public string? ImageImportDirectory { get; private set; }

        /// <summary>
        /// Gets the path to the image export directory. Defaults to <c>%LOCALAPPDATA%\RadioExt-Helper\tools\exported</c>.
        /// </summary>
        public string? ImageExportDirectory { get; private set; }

        /// <summary>
        /// Gets the path to the imported working directory. Defaults to <c>%LOCALAPPDATA%\RadioExt-Helper\tools\imported_working_directory</c>.
        /// </summary>
        public string? ImportedWorkingDirectory { get; private set; }

        /// <summary>
        /// Gets the path to the extracted working directory. Defaults to <c>%LOCALAPPDATA%\RadioExt-Helper\tools\extracted_working_directory</c>.
        /// </summary>
        public string? ExtractedWorkingDirectory { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the icon manager has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }
        #endregion

        private IconManager()
        {
            try
            {
                if (_instance != null)
                    throw new InvalidOperationException("An instance of IconManager already exists. Use IconManager.Instance to access it.");

                _cancellationTokenSource = new CancellationTokenSource();

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

        /// <summary>
        /// Disposes the resources used by the <see cref="IconManager"/>.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            CleanupResources();
        }

        /// <summary>
        /// Deletes the temporary directories and resets the paths for the icon generator.
        /// </summary>
        public void ClearTempData() => TearDownPaths();

        /// <summary>
        /// Cleans up any resources used by the icon manager. This mainly involves deleting the temporary directory: <see cref="WolvenKitTempDirectory"/>.
        /// </summary>
        private void CleanupResources()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                if (Directory.Exists(WolvenKitTempDirectory))
                    Directory.Delete(WolvenKitTempDirectory, true);
            }
            catch (Exception e)
            {
                AuLogger.GetCurrentLogger<IconManager>("CleanupResources").Error(e.Message);
            }
        }

        /// <summary>
        /// Initializes the icon manager. This method sets up the required paths and downloads the WolvenKit CLI if required. Only needs to be called once.
        /// </summary>
        /// <returns>A task representing the asynchronous initialization operation.</returns>
        public async Task InitializeAsync()
        {
            if (IsInitialized) return;

            try
            {
                SetupRequiredPaths();

                _wolvenKitCliDownloadUrl = $"https://github.com/WolvenKit/WolvenKit/releases/download/{WolvenKitVersion}/WolvenKit.Console-{WolvenKitVersion}.zip";

                if (WolvenKitTempDirectory == null)
                    throw new InvalidOperationException("The WolvenKit temp directory is null.");

                _wolvenKitCliExe = Path.Combine(WolvenKitTempDirectory, "WolvenKit.CLI.exe");

                await DownloadWolvenKitIfRequiredAsync();

                if (!File.Exists(_wolvenKitCliExe))
                    throw new FileNotFoundException("The WolvenKit CLI executable could not be found.", _wolvenKitCliExe);

                _wolvenKitCli = new Cli(_wolvenKitCliExe, _cancellationTokenSource!.Token);

                // Subscribe to CLI events
                _wolvenKitCli.OutputChanged += (sender, output) => OnCliProgressChanged(output);
                _wolvenKitCli.ErrorChanged += (sender, error) => OnCliErrorOccurred(error);

                IsInitialized = true;
            }
            catch (Exception e)
            {
                AuLogger.GetCurrentLogger<IconManager>("InitializeAsync").Error(e.Message);
            }
        }

        private void OnCliErrorOccurred(string? error)
        {
            if (error == null) return;

            CliStatus?.Invoke(this, new StatusEventArgs(error, true, _currentProgress));
        }

        private void OnCliProgressChanged(string? output)
        {
            if (output == null) return;

            CliStatus?.Invoke(this, new StatusEventArgs(output, false, _currentProgress));
        }

        /// <summary>
        /// Sets up the required directories for the icon generator.
        /// </summary>
        private void SetupRequiredPaths()
        {
            try
            {
                WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Wolven Icon Generator", "tools");
                WolvenKitTempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                ImageImportDirectory = Path.Combine(WorkingDirectory, "imported_pngs");
                ImageExportDirectory = Path.Combine(WorkingDirectory, "extracted_pngs");
                ImportedWorkingDirectory = Path.Combine(WorkingDirectory, "imported_working_directory");
                ExtractedWorkingDirectory = Path.Combine(WorkingDirectory, "extracted_working_directory");

                Directory.CreateDirectory(WorkingDirectory);
                Directory.CreateDirectory(WolvenKitTempDirectory);
                Directory.CreateDirectory(ImageImportDirectory);
                Directory.CreateDirectory(ImageExportDirectory);
                Directory.CreateDirectory(ImportedWorkingDirectory);
                Directory.CreateDirectory(ExtractedWorkingDirectory);
            }
            catch (Exception e)
            {
                AuLogger.GetCurrentLogger<IconManager>("SetupRequiredPaths").Error(e, e.Message);
            }
        }

        /// <summary>
        /// Deletes the temporary directories and resets the paths for the icon generator.
        /// </summary>
        private void TearDownPaths()
        {
            try
            {
                if (Directory.Exists(WolvenKitTempDirectory))
                    Directory.Delete(WolvenKitTempDirectory, true);

                if (Directory.Exists(ImageImportDirectory))
                    Directory.Delete(ImageImportDirectory, true);

                if (Directory.Exists(ImageExportDirectory))
                    Directory.Delete(ImageExportDirectory, true);

                if (Directory.Exists(ImportedWorkingDirectory))
                    Directory.Delete(ImportedWorkingDirectory, true);

                if (Directory.Exists(ExtractedWorkingDirectory))
                    Directory.Delete(ExtractedWorkingDirectory, true);

                IsInitialized = false;
                SetupRequiredPaths();
            }
            catch (Exception e)
            {
                AuLogger.GetCurrentLogger<IconManager>("TearDownPaths").Error(e, e.Message);
            }
        }

        #region Creating a Faux Project

        /// <summary>
        /// Creates the necessary directories for the import operation based on the atlas name. Optionally, overwrite the existing directories.
        /// </summary>
        /// <param name="atlasName">The name that icon atlas will be generated with.</param>
        /// <param name="overwrite">Indicates whether existing directories should be overwritten if they exist.</param>
        /// <returns>A dictionary where the key is the type of project folder and the value is the folder path.</returns>
        private Dictionary<string, string> CreateImportDirectories(string atlasName, bool overwrite = false)
        {
            try
            {
                if (WorkingDirectory == null)
                    throw new InvalidOperationException("The working directory is null.");

                if (ImportedWorkingDirectory == null)
                    throw new InvalidOperationException("The imported working directory is null.");

                if (ImageImportDirectory == null)
                    throw new InvalidOperationException("The image import directory is null.");

                var outputDictionary = new Dictionary<string, string>();
                var guid = Guid.NewGuid();

                // Create the path that imported PNGs are stored
                var importedPngsPath = Path.Combine(ImageImportDirectory, $"{atlasName}-{guid}");
                if (overwrite && Directory.Exists(importedPngsPath))
                    Directory.Delete(importedPngsPath, true);
                Directory.CreateDirectory(importedPngsPath);
                outputDictionary["importedPngs"] = importedPngsPath;

                // Create the base path for the project
                var projectBasePath = Path.Combine(ImportedWorkingDirectory, $"{atlasName}-{guid}");
                if (overwrite && Directory.Exists(projectBasePath))
                    Directory.Delete(projectBasePath, true);
                Directory.CreateDirectory(projectBasePath);
                outputDictionary["projectBasePath"] = projectBasePath;

                // Create the base path for the REDEngine files
                var archiveBasePath = Path.Combine(projectBasePath, "source", "archive");
                if (overwrite && Directory.Exists(archiveBasePath))
                    Directory.Delete(archiveBasePath, true);
                Directory.CreateDirectory(archiveBasePath);
                outputDictionary["archiveBasePath"] = archiveBasePath;

                // Create the path to the REDEngine files
                var redEngineFilesPath = Path.Combine(archiveBasePath, "base", "icon");
                if (overwrite && Directory.Exists(redEngineFilesPath))
                    Directory.Delete(redEngineFilesPath, true);
                Directory.CreateDirectory(redEngineFilesPath);
                outputDictionary["redEngineFilesPath"] = redEngineFilesPath;

                // Create the base path for the raw files
                var rawFilesBasePath = Path.Combine(projectBasePath, "source", "raw");
                if (overwrite && Directory.Exists(rawFilesBasePath))
                    Directory.Delete(rawFilesBasePath, true);
                Directory.CreateDirectory(rawFilesBasePath);
                outputDictionary["rawFilesBasePath"] = rawFilesBasePath;

                // Create the path to the raw (non-REDEngine) files
                var rawFilesPath = Path.Combine(rawFilesBasePath, "base", "icon");
                if (overwrite && Directory.Exists(rawFilesPath))
                    Directory.Delete(rawFilesPath, true);
                Directory.CreateDirectory(rawFilesPath);
                outputDictionary["rawFilesPath"] = rawFilesPath;

                outputDictionary["iconGuid"] = guid.ToString();

                return outputDictionary;
            }
            catch (Exception e)
            {
                AuLogger.GetCurrentLogger<IconManager>("CreateImportDirectories").Error(e, e.Message);
            }

            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Creates the necessary directories for the export operation based on the atlas name. Optionally, overwrite the existing directories.
        /// </summary>
        /// <param name="atlasName">The name that icon atlas will be generated with.</param>
        /// <param name="overwrite">Indicates whether existing directories should be overwritten if they exist.</param>
        /// <returns>A dictionary where the key is the type of project folder and the value is the folder path.</returns>
        private Dictionary<string, string> CreateExportDirectories(string atlasName, bool overwrite = false)
        {
            try
            {
                if (WorkingDirectory == null)
                    throw new InvalidOperationException("The working directory is null.");

                if (ExtractedWorkingDirectory == null)
                    throw new InvalidOperationException("The extracted working directory is null.");

                if (ImageExportDirectory == null)
                    throw new InvalidOperationException("The image export directory is null.");

                var outputDictionary = new Dictionary<string, string>();

                // Create the path that exported PNGs are stored
                var guid = Guid.NewGuid();
                var exportedPngsPath = Path.Combine(ImageExportDirectory, $"{atlasName}-{guid}");
                if (overwrite && Directory.Exists(exportedPngsPath))
                    Directory.Delete(exportedPngsPath, true);
                Directory.CreateDirectory(exportedPngsPath);
                outputDictionary["exportedPngs"] = exportedPngsPath;

                // Create the base path for the project
                var projectBasePath = Path.Combine(ExtractedWorkingDirectory, $"{atlasName}-{guid}");
                if (overwrite && Directory.Exists(projectBasePath))
                    Directory.Delete(projectBasePath, true);
                Directory.CreateDirectory(projectBasePath);
                outputDictionary["projectBasePath"] = projectBasePath;

                outputDictionary["iconGuid"] = guid.ToString();

                return outputDictionary;
            }
            catch (Exception e)
            {
                AuLogger.GetCurrentLogger<IconManager>("CreateExportDirectories").Error(e, e.Message);
            }

            return new Dictionary<string, string>();
        }
        #endregion

        #region WolvenKit Download and Extraction

        /// <summary>
        /// Downloads the WolvenKit CLI if it has not been downloaded yet and extracts it to the temporary directory: <see cref="WolvenKitTempDirectory"/>.
        /// </summary>
        /// <returns>A task representing the asynchronous download and extraction operation.</returns>
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
                AuLogger.GetCurrentLogger<IconManager>("DownloadWolvenKitIfRequiredAsync").Error(e.Message);
            }
        }

        /// <summary>
        /// Asynchronously downloads the WolvenKit CLI from the specified URL.
        /// </summary>
        /// <param name="zipFile">The zip file to save.</param>
        /// <returns>A task representing the asynchronous download operation.</returns>
        private async Task<bool> DownloadWolvenKitAsync(string zipFile)
        {
            if (string.IsNullOrEmpty(_wolvenKitCliDownloadUrl))
                throw new InvalidOperationException("The WolvenKit CLI download URL is null or empty.");

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

        #region Import - Creating .archive Files

        /// <summary>
        /// Asynchronously generates a <c>.archive</c> file from a PNG image.
        /// This method supports progress reporting and task cancellation.
        /// </summary>
        /// <param name="imagePath">The path to the image file on disk.</param>
        /// <param name="atlasName">The name that icon atlas will be generated with.</param>
        /// <param name="progress">An optional progress reporter to track the progress of the operation.</param>
        /// <param name="overwrite">Indicates whether existing directories should be overwritten if they exist.</param>
        /// <param name="cancellationToken">The cancellation token for stopping the task.</param>
        /// <returns>A task that, when complete, returns a <see cref="WolvenIcon"/> or <c>null</c> if the operation is canceled or fails.</returns>
        /// <exception cref="InvalidOperationException">Occurs when the icon manager has not been initialized.</exception>
        public async Task<WolvenIcon?> GenerateIconImageAsync(string imagePath, string atlasName, IProgress<int>? progress = null, bool overwrite = true, CancellationToken cancellationToken = default)
        {
            _currentProgress = 0;
            if (!IsInitialized)
                throw new InvalidOperationException("The icon manager has not been initialized.");

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var token = _cancellationTokenSource.Token;

            try
            {
                AuLogger.GetCurrentLogger<IconManager>("GenerateIconImageAsync").Info($"The icon import operation has started: {atlasName}");
                OnIconImportStarted(new StatusEventArgs($"The icon import operation has started: {atlasName}", false, _currentProgress));

                WolvenIcon? icon = null;

                // Task.Run allows async cancellation support inside the method
                await Task.Run(async () =>
                {
                    icon = await CreateIconAsync(imagePath, atlasName, overwrite, token, progress);
                    if (icon == null)
                        throw new InvalidOperationException("The icon could not be created.");
                }, token);

                AuLogger.GetCurrentLogger<IconManager>("GenerateIconImageAsync").Info($"The icon import operation has completed successfully: {icon?.AtlasName}");
                OnIconImportFinished(new StatusEventArgs($"The icon import operation has completed successfully: {icon?.AtlasName}", false, _currentProgress));

                return icon;
            }
            catch (OperationCanceledException)
            {
                AuLogger.GetCurrentLogger<IconManager>("GenerateIconImageAsync").Info($"The icon import operation was cancelled: {atlasName}");
                OnIconImportStatus(new StatusEventArgs($"The icon import operation was cancelled: {atlasName}", false, _currentProgress));
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
        /// Asynchronously creates a <see cref="WolvenIcon"/> object from the specified image path and atlas name.
        /// This method supports cancellation and progress reporting.
        /// </summary>
        /// <param name="imagePath">The path to the PNG file to create the icon from.</param>
        /// <param name="atlasName">The name for the atlas. Must be lowercase.</param>
        /// <param name="overwrite">Indicates whether existing files should be overwritten.</param>
        /// <param name="token">The token used for cancelling operations.</param>
        /// <param name="progress">An optional progress reporter for reporting progress.</param>
        /// <returns>A task containing the resulting <see cref="WolvenIcon"/>.</returns>
        private async Task<WolvenIcon> CreateIconAsync(string imagePath, string atlasName, bool overwrite, CancellationToken token, IProgress<int>? progress)
        {
            try
            {
                // Initial validation
                _currentProgress += 5;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                if (!File.Exists(imagePath))
                {
                    OnIconImportStatus(new StatusEventArgs("The image file could not be found.", true, _currentProgress));
                    CancelOperation();
                    throw new FileNotFoundException("The image file could not be found.", imagePath);
                }

                _currentProgress += 5;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                if (!ImageUtils.IsPngFile(imagePath))
                {
                    OnIconImportStatus(new StatusEventArgs("The image file is not a PNG file.", true, _currentProgress));
                    CancelOperation();
                    throw new InvalidDataException("The image file is not a PNG file.");
                }

                _currentProgress += 5;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                // Create necessary directories
                var projectDirectories = CreateImportDirectories(atlasName, overwrite);
                if (projectDirectories.Count == 0)
                {
                    OnIconImportStatus(new StatusEventArgs("The project directories could not be created.", true, _currentProgress));
                    CancelOperation();
                    throw new InvalidOperationException("The project directories could not be created.");
                }

                _currentProgress += 10;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                // Copy the image to the images folder for the project
                var imageFileName = Path.GetFileName(imagePath);
                var projectImagePath = Path.Combine(projectDirectories["importedPngs"], imageFileName);
                File.Copy(imagePath, projectImagePath, overwrite);

                _currentProgress += 10;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                // Use InkAtlasGenerator to generate the .inkatlas.json and images
                await _inkAtlasGenerator!.GenerateInkAtlasJsonAsync(projectDirectories["importedPngs"], projectDirectories["rawFilesPath"], atlasName, token, progress);

                _currentProgress += 10;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                // Convert to InkAtlas file
                await _wolvenKitCli!.ConvertToInkAtlasFileAsync(projectDirectories["rawFilesPath"], token, progress);

                _currentProgress += 10;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                // Import to WolvenKit project
                await _wolvenKitCli.ImportToWolvenKitProjectAsync(projectDirectories["rawFilesPath"], token, progress);

                _currentProgress += 10;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                // Copy project files
                CopyProjectFiles(projectDirectories["rawFilesPath"], projectDirectories["redEngineFilesPath"]);

                _currentProgress += 10;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                // Pack the project into a .archive file
                await _wolvenKitCli.PackArchiveAsync(projectDirectories["archiveBasePath"], projectDirectories["projectBasePath"], token, progress);

                // Rename the .archive file to the atlas name
                var originalArchivePath = Path.Combine(projectDirectories["projectBasePath"], "archive.archive");
                var newArchivePath = Path.Combine(projectDirectories["projectBasePath"], $"{atlasName}.archive");
                File.Move(originalArchivePath, newArchivePath, overwrite);

                _currentProgress += 10;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                // Verify the new archive exists
                if (!File.Exists(newArchivePath))
                {
                    OnIconImportStatus(new StatusEventArgs("The final .archive file could not be renamed.", true, _currentProgress));
                    CancelOperation();
                    throw new FileNotFoundException("The final .archive file could not be renamed.", newArchivePath);
                }

                _currentProgress += 5;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                // Delete the original archive
                File.Delete(originalArchivePath);

                // Create and return the WolvenIcon object
                var icon = new WolvenIcon(projectImagePath)
                {
                    CustomIcon =
                    {
                        InkAtlasPath = Path.Combine("base", "icon", $"{atlasName}.inkatlas"),
                        InkAtlasPart = "icon_part"
                    },
                    AtlasName = atlasName,
                    OriginalArchivePath = newArchivePath,
                    Sha256HashOfArchiveFile = HashUtils.ComputeSha256Hash(newArchivePath, true),
                    IconId = Guid.Parse(projectDirectories["iconGuid"])
                };

                _currentProgress = 100;
                progress?.Report(_currentProgress);
                return icon;
            }
            catch (Exception e)
            {
                AuLogger.GetCurrentLogger<IconManager>("CreateIconAsync").Error(e, e.Message);
                throw;
            }
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

        #region Export - Extracting .archive Files

        /// <summary>
        /// Asynchronously extracts (unbundles) the icon from the specified .archive file.
        /// This method supports progress reporting and task cancellation.
        /// </summary>
        /// <param name="archivePath">The path to a .archive file to unbundle.</param>
        /// <param name="progress">An optional progress reporter to track the progress of the operation.</param>
        /// <param name="overwrite">Indicates whether existing directories should be overwritten if they exist.</param>
        /// <param name="cancellationToken">The cancellation token for stopping the task.</param>
        /// <returns>A task that, when complete, returns a <see cref="WolvenIcon"/> or <c>null</c> if the operation is canceled or fails.</returns>
        /// <exception cref="InvalidOperationException">Occurs when the icon manager has not been initialized.</exception>
        public async Task<WolvenIcon?> ExtractIconImageAsync(string archivePath, IProgress<int>? progress = null, bool overwrite = true, CancellationToken cancellationToken = default)
        {
            _currentProgress = 0;
            if (!IsInitialized)
                throw new InvalidOperationException("The icon manager has not been initialized.");

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var token = _cancellationTokenSource.Token;

            try
            {
                AuLogger.GetCurrentLogger<IconManager>("ExtractIconImageAsync").Info($"The icon export operation has started: {archivePath}");
                OnIconExportStarted(new StatusEventArgs($"The icon export operation has started: {archivePath}", false, _currentProgress));

                WolvenIcon? icon = null;

                await Task.Run(async () =>
                {
                    icon = await ExtractIconAsync(archivePath, overwrite, token, progress);
                    if (icon == null)
                        throw new InvalidOperationException("The icon could not be extracted.");
                }, token);

                AuLogger.GetCurrentLogger<IconManager>("ExtractIconImageAsync").Info($"The icon export operation has completed successfully: {icon?.AtlasName}");
                OnIconExportFinished(new StatusEventArgs($"The icon export operation has completed successfully: {icon?.AtlasName}", false, _currentProgress));

                return icon;
            }
            catch (OperationCanceledException)
            {
                AuLogger.GetCurrentLogger<IconManager>("ExtractIconImageAsync").Info($"The icon export operation was cancelled: {archivePath}");
                OnIconExportStatus(new StatusEventArgs($"The icon export operation was cancelled: {archivePath}", false, _currentProgress));
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
        /// Asynchronously extracts a <see cref="WolvenIcon"/> object from the specified .archive file.
        /// This method supports cancellation and progress reporting.
        /// </summary>
        /// <param name="archivePath">The path to a .archive file to unbundle.</param>
        /// <param name="overwrite">Indicates whether existing directories should be overwritten if they exist.</param>
        /// <param name="token">The token used for cancelling operations.</param>
        /// <param name="progress">An optional progress reporter for reporting progress.</param>
        /// <returns>A task containing the resulting <see cref="WolvenIcon"/>.</returns>
        private async Task<WolvenIcon> ExtractIconAsync(string archivePath, bool overwrite, CancellationToken token, IProgress<int>? progress)
        {
            try
            {
                // Initial validation
                _currentProgress += 5;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                if (!File.Exists(archivePath))
                {
                    OnIconExportStatus(new StatusEventArgs("The archive file could not be found.", true, _currentProgress));
                    CancelOperation();
                    throw new FileNotFoundException("The archive file could not be found.", archivePath);
                }

                _currentProgress += 5;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                var atlasName = Path.GetFileNameWithoutExtension(archivePath);

                // Create necessary directories
                var projectDirectories = CreateExportDirectories(atlasName, overwrite);
                if (projectDirectories.Count == 0)
                {
                    OnIconExportStatus(new StatusEventArgs("The project directories could not be created.", true, _currentProgress));
                    CancelOperation();
                    throw new InvalidOperationException("The project directories could not be created.");
                }

                _currentProgress += 10;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                // Unpack the archive file
                await _wolvenKitCli!.UnpackArchiveAsync(archivePath, projectDirectories["projectBasePath"], token, progress);

                _currentProgress += 10;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                // Convert the XBM files to PNG
                var xbmFiles = Directory.GetFiles(projectDirectories["projectBasePath"], "*.xbm", SearchOption.AllDirectories);
                if (xbmFiles.Length == 0)
                {
                    OnIconExportStatus(new StatusEventArgs("No XBM files were found in the project folder.", true, _currentProgress));
                    CancelOperation();
                    throw new FileNotFoundException("No XBM files were found in the project folder.", archivePath);
                }

                _currentProgress += 10;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                foreach (var xbmFile in xbmFiles)
                {
                    await _wolvenKitCli.ExportPngAsync(xbmFile, projectDirectories["exportedPngs"], token, progress);
                    _currentProgress += 5;
                    progress?.Report(_currentProgress);
                    token.ThrowIfCancellationRequested();
                }

                // Get the PNG files from the exported folder
                var pngFiles = Directory.GetFiles(projectDirectories["exportedPngs"], "*.png");

                // Rename the .archive file to the atlas name
                var newArchivePath = Path.Combine(projectDirectories["projectBasePath"], $"{atlasName}.archive");
                File.Copy(archivePath, newArchivePath, overwrite);

                _currentProgress += 10;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                // Verify the new archive exists
                if (!File.Exists(newArchivePath))
                {
                    OnIconExportStatus(new StatusEventArgs("The final .archive file could not be renamed.", true, _currentProgress));
                    CancelOperation();
                    throw new FileNotFoundException("The final .archive file could not be renamed.", newArchivePath);
                }

                // Ensure there is at least one PNG
                if (pngFiles.Length == 0)
                {
                    OnIconExportStatus(new StatusEventArgs("No PNG files were found in the exported folder.", true, _currentProgress));
                    CancelOperation();
                    throw new FileNotFoundException("No PNG files were found in the exported folder.", projectDirectories["exportedPngs"]);
                }

                // Get the InkAtlas path and part
                var pathAndParts = await GetInkAtlasPathAndPartAsync(projectDirectories["projectBasePath"], xbmFiles.First(), token, progress);

                _currentProgress += 10;
                progress?.Report(_currentProgress);
                token.ThrowIfCancellationRequested();

                if (string.IsNullOrEmpty(pathAndParts.inkAtlasPath) || string.IsNullOrEmpty(pathAndParts.inkAtlasPart))
                {
                    OnIconExportStatus(new StatusEventArgs("The .inkatlas path and part could not be determined.", true, _currentProgress));
                    CancelOperation();
                    throw new InvalidOperationException("The .inkatlas path and part could not be determined.");
                }

                // Create and return the WolvenIcon object
                var icon = new WolvenIcon(pngFiles.First(), newArchivePath)
                {
                    CustomIcon =
                    {
                        InkAtlasPath = pathAndParts.inkAtlasPath,
                        InkAtlasPart = pathAndParts.inkAtlasPart
                    },
                    AtlasName = atlasName,
                    OriginalArchivePath = archivePath,
                    IsFromArchive = true,
                    IconId = Guid.Parse(projectDirectories["iconGuid"])
                };

                _currentProgress = 100;
                progress?.Report(_currentProgress);
                return icon;
            }
            catch (Exception e)
            {
                AuLogger.GetCurrentLogger<IconManager>("ExtractIconAsync").Error(e, e.Message);
                throw;
            }
        }

        private async Task<(string inkAtlasPath, string inkAtlasPart)> GetInkAtlasPathAndPartAsync(string projectBasePath, string xbmFile, CancellationToken token, IProgress<int>? progress)
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

                await _wolvenKitCli.ConvertToInkAtlasJsonFileAsync(inkAtlasFile, token);

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
        /// Resets the current progress percentage to 0.
        /// </summary>
        private void ResetProgress()
        {
            _currentProgress = 0;
        }

        /// <summary>
        /// Cancels the currently running operation.
        /// </summary>
        public void CancelOperation()
        {
            _cancellationTokenSource?.Cancel();
        }

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
}