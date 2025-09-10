// // AudioManager.cs : WolvenIconGenerator
// // Copyright (C) 2025  Ethan Hann
// //
// // This program is free software: you can redistribute it and/or modify
// // it under the terms of the GNU General Public License as published by
// // the Free Software Foundation, either version 3 of the License, or
// // (at your option) any later version.
// //
// // This program is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// // GNU General Public License for more details.
// //
// // You should have received a copy of the GNU General Public License
// // along with this program.  If not, see <https://www.gnu.org/licenses/>.

#region

using AetherUtils.Core.Files;
using AetherUtils.Core.Logging;
using WIG.Lib.Models;
using WIG.Lib.Models.Audio;
using WIG.Lib.Services;
using WIG.Lib.Tools.InkAtlas;

#endregion

namespace WIG.Lib.Utility;

/// <summary>
///     Manager class responsible for handling audio-related functionalities.
///     This class is a singleton, ensuring only one instance exists throughout the application.
///     <para>This manager can create <c>.archive</c> files (import) from raw audio files.</para>
///     <para>All long-running operations support cancellation via the <see cref="CancelOperation" /> method.</para>
///     <para>Events can be subscribed to in order to track the progress of running operations.</para>
/// </summary>
public class AudioManager : IDisposable
{
    private static readonly object Lock = new();
    private static AudioManager? _instance;

    private CancellationTokenSource? _cancellationTokenSource;

    private int _currentProgress;

    private AudioManager()
    {
        try
        {
            if (_instance != null)
                throw new InvalidOperationException(
                    "AudioManager instance already exists. Use AudioManager.Instance to access the singleton instance.");

            _cancellationTokenSource = new CancellationTokenSource();
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<AudioManager>("Constructor").Error(ex);
        }
    }

    /// <summary>
    ///     Gets the singleton instance of the <see cref="AudioManager" />.
    /// </summary>
    public static AudioManager Instance
    {
        get
        {
            lock (Lock)
            {
                return _instance ??= new AudioManager();
            }
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        CleanupResources();
    }

    /// <summary>
    ///     Occurs when the audio import operation is started.
    /// </summary>
    public event EventHandler<StatusEventArgs>? AudioImportStarted;

    /// <summary>
    ///     Occurs when the audio import operation is in progress.
    /// </summary>
    public event EventHandler<StatusEventArgs>? AudioImportFinished;

    /// <summary>
    ///     Event that is raised when the status of the CLI changes.
    /// </summary>
    public event EventHandler<StatusEventArgs>? CliStatus;

    /// <summary>
    ///     Initializes the AudioManager by setting up necessary directories and fetching vanilla station data.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        if (IsInitialized) return;

        try
        {
            //Make sure the audio manager was initialized. It should have been initialized before this and contains the WolvenKit files.
            if (!AudioManager.Instance.IsInitialized)
                throw new InvalidOperationException("AudioManager must be initialized before AudioManager.");

            SetupRequiredPaths();

            if (WorkingDirectory == null)
                throw new InvalidOperationException("The working directory is null.");

            // Fetch and save the vanilla stations JSON file
            var jsonOutputPath = Path.Combine(WorkingDirectory, "vanillaStations.json");
            var fetcher = new FetchVanillaStations(RadioStationsSheetURL, jsonOutputPath);

            VanillaStations = await fetcher.FetchAndSaveAsync();

            IsInitialized = VanillaStations.Count != 0;
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<AudioManager>("InitializeAsync").Error(ex);
        }
    }

    private void SetupRequiredPaths()
    {
        try
        {
            WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Wolven audio Generator", "tools", "audio");
            AudioImportDirectory = Path.Combine(WorkingDirectory, "imported");
            ImportedWorkingDirectory = Path.Combine(WorkingDirectory, "imported_working_directory");
            WolvenKitTempDirectory = AudioManager.Instance.WolvenKitTempDirectory;

            Directory.CreateDirectory(WorkingDirectory);
            Directory.CreateDirectory(AudioImportDirectory);
            Directory.CreateDirectory(ImportedWorkingDirectory);
            //No need to create WolvenKitTempDirectory, AudioManager should have already created it.

            AuLogger.GetCurrentLogger<AudioManager>("SetupRequiredPaths").Info($"WorkingDirectory: {WorkingDirectory}");
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<AudioManager>("SetupRequiredPaths").Error(ex);
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

    private void OnAudioImportStarted(StatusEventArgs e)
    {
        AudioImportStarted?.Invoke(this, e);
    }

    private void OnAudioImportStatus(StatusEventArgs e)
    {
        CliStatus?.Invoke(this, e);
    }

    private void OnAudioImportFinished(StatusEventArgs e)
    {
        AudioImportFinished?.Invoke(this, e);
    }

    ~AudioManager()
    {
        CleanupResources();
    }

    private void CleanupResources()
    {
        try
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<AudioManager>("CleanupResources").Error(ex);
        }
    }

    /// <summary>
    ///     Creates the necessary directories for the import operation based on the station name.
    ///     Optionally, overwrite the existing directories.
    /// </summary>
    /// <param name="stationName">The name of the station to overwrite in game.</param>
    /// <param name="overwrite">Indicates whether existing directories should be overwritten if they exist.</param>
    /// <returns>A dictionary where the key is the type of project folder and the value is the folder path.</returns>
    private Dictionary<string, string> CreateImportDirectories(string stationName, bool overwrite = false)
    {
        try
        {
            if (ImportedWorkingDirectory == null)
                throw new InvalidOperationException("The imported working directory is null.");

            if (AudioImportDirectory == null)
                throw new InvalidOperationException("The audio import directory is null.");

            Dictionary<string, string> outputDictionary = new();
            var guid = Guid.NewGuid();

            // Create the path that imported PNGs are stored
            var importedAudioPath = Path.Combine(AudioImportDirectory, $"{stationName}-{guid}");
            if (overwrite && Directory.Exists(importedAudioPath))
                Directory.Delete(importedAudioPath, true);
            Directory.CreateDirectory(importedAudioPath);
            outputDictionary["importedAudio"] = importedAudioPath;

            // Create the base path for the project
            var projectBasePath = Path.Combine(ImportedWorkingDirectory, $"{stationName}-{guid}");
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

            outputDictionary["replacedStationGuid"] = guid.ToString();

            return outputDictionary;
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<AudioManager>("CreateImportDirectories").Error(e, e.Message);
        }

        return new Dictionary<string, string>();
    }

    /// <summary>
    /// Asynchronously generates a <c>.archive</c> file from a list of audio files.
    /// This method supports progress reporting and task cancellation.
    /// </summary>
    /// <param name="vanillaStationName">The original, vanilla station to retrieve filenames from.</param>
    /// <param name="files">The list of files to use as replacement tracks.</param>
    /// <param name="progress">An optional progress reporter to track the progress of the operation.</param>
    /// <param name="overwrite">Indicates whether existing directories should be overwritten if they exist.</param>
    /// <param name="cancellationToken">The cancellation token for stopping the task.</param>
    /// <returns>A task that, when complete, returns a <see cref="WolvenIcon"/> or <c>null</c> if the operation is canceled or fails.</returns>
    /// <exception cref="InvalidOperationException">Occurs when the audio manager has not been initialized.</exception>
    public async Task<WolvenAudio?> GenerateAudioArchiveAsync(string vanillaStationName, List<ReplacementTrack> files,
        IProgress<int>? progress = null, bool overwrite = true, CancellationToken cancellationToken = default)
    {
        _currentProgress = 0;
        if (!IsInitialized)
            throw new InvalidOperationException("The audio manager has not been initialized.");

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        CancellationToken token = _cancellationTokenSource.Token;

        try
        {
            AuLogger.GetCurrentLogger<AudioManager>("GenerateIconImageAsync")
                .Info($"The audio import operation has started: {vanillaStationName}");
            OnAudioImportStarted(new StatusEventArgs($"The audio import operation has started: {vanillaStationName}", false,
                _currentProgress));

            WolvenAudio? audio = null;

            // Task.Run allows async cancellation support inside the method
            await Task.Run(async () =>
            {
                audio = await CreateAudioArchiveAsync(imagePath, vanillaStationName, overwrite, token, progress);
                if (audio == null)
                    throw new InvalidOperationException("The audio could not be created.");
            }, token);

            AuLogger.GetCurrentLogger<AudioManager>("GenerateIconImageAsync")
                .Info($"The audio import operation has completed successfully: {audio?.vanillaStationName}");
            OnIconImportFinished(new StatusEventArgs(
                $"The audio import operation has completed successfully: {audio?.vanillaStationName}", false, _currentProgress));

            return audio;
        }
        catch (OperationCanceledException)
        {
            AuLogger.GetCurrentLogger<AudioManager>("GenerateIconImageAsync")
                .Info($"The audio import operation was cancelled: {vanillaStationName}");
            OnIconImportStatus(new StatusEventArgs($"The audio import operation was cancelled: {vanillaStationName}", false,
                _currentProgress));
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<AudioManager>("GenerateIconImageAsync").Error(e, e.Message);
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
    /// <param name="vanillaStationName">The path to the PNG file to create the audio from.</param>
    /// <param name="files">The name for the atlas. Must be lowercase.</param>
    /// <param name="overwrite">Indicates whether existing files should be overwritten.</param>
    /// <param name="token">The token used for cancelling operations.</param>
    /// <param name="progress">An optional progress reporter for reporting progress.</param>
    /// <returns>A task containing the resulting <see cref="WolvenIcon"/>.</returns>
    private async Task<WolvenIcon> CreateAudioArchiveAsync(string vanillaStationName, List<ReplacementTrack> files, bool overwrite,
        CancellationToken token, IProgress<int>? progress)
    {
        try
        {
            // Initial validation
            _currentProgress += 5;
            progress?.Report(_currentProgress);
            token.ThrowIfCancellationRequested();

            //Check files exist in the replacement tracks list
            foreach (var file in files.Where(file => !FileHelper.DoesFileExist(file.ReplacementFilePath)))
            {
                OnAudioImportStarted(new StatusEventArgs($"The audio file could not be found: {file.ReplacementFilePath}", true, _currentProgress));
                CancelOperation();
                throw new FileNotFoundException("The image file could not be found.", file.ReplacementFilePath);
            }

            _currentProgress += 10;
            progress?.Report(_currentProgress);
            token.ThrowIfCancellationRequested();

            // Create necessary directories
            var projectDirectories = CreateImportDirectories(vanillaStationName, overwrite);
            if (projectDirectories.Count == 0)
            {
                OnAudioImportStarted(new StatusEventArgs("The project directories could not be created.", true,
                    _currentProgress));
                CancelOperation();
                throw new InvalidOperationException("The project directories could not be created.");
            }

            _currentProgress += 10;
            progress?.Report(_currentProgress);
            token.ThrowIfCancellationRequested();

            // Copy the audio files to the audio folder for the project
            foreach (var file in files)
            {
                var trackFileName = Path.GetFileName(file.ReplacementFilePath);
                var projectImagePath = Path.Combine(projectDirectories["importedAudio"], trackFileName);
                File.Copy(file.ReplacementFilePath, projectImagePath, overwrite);
            }

            _currentProgress += 10;
            progress?.Report(_currentProgress);
            token.ThrowIfCancellationRequested();

            // Use InkAtlasGenerator to generate the .inkatlas.json and images
            await _inkAtlasGenerator!.GenerateInkAtlasJsonAsync(projectDirectories["importedPngs"],
                projectDirectories["rawFilesPath"], atlasName, token, progress);

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
            await _wolvenKitCli.PackArchiveAsync(projectDirectories["archiveBasePath"],
                projectDirectories["projectBasePath"], token, progress);

            // Rename the .archive file to the atlas name
            string originalArchivePath = Path.Combine(projectDirectories["projectBasePath"], "archive.archive");
            string newArchivePath = Path.Combine(projectDirectories["projectBasePath"], $"{atlasName}.archive");
            File.Move(originalArchivePath, newArchivePath, overwrite);

            _currentProgress += 10;
            progress?.Report(_currentProgress);
            token.ThrowIfCancellationRequested();

            // Verify the new archive exists
            if (!File.Exists(newArchivePath))
            {
                OnIconImportStatus(new StatusEventArgs("The final .archive file could not be renamed.", true,
                    _currentProgress));
                CancelOperation();
                throw new FileNotFoundException("The final .archive file could not be renamed.", newArchivePath);
            }

            _currentProgress += 5;
            progress?.Report(_currentProgress);
            token.ThrowIfCancellationRequested();

            // Delete the original archive
            File.Delete(originalArchivePath);

            // Create and return the WolvenIcon object
            WolvenIcon audio = new(projectImagePath)
            {
                CustomIcon =
                {
                    InkAtlasPath = Path.Combine("base", "audio", $"{atlasName}.inkatlas"),
                    InkAtlasPart = "icon_part"
                },
                AtlasName = atlasName,
                OriginalArchivePath = newArchivePath,
                Sha256HashOfArchiveFile = HashUtils.ComputeSha256Hash(newArchivePath, true),
                IconId = Guid.Parse(projectDirectories["iconGuid"])
            };

            _currentProgress = 100;
            progress?.Report(_currentProgress);
            return audio;
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<IconManager>("CreateAudioArchiveAsync").Error(e, e.Message);
            throw;
        }
    }

    /// <summary>
    /// Get a vanilla station by its name, case-insensitive.
    /// </summary>
    /// <param name="stationName">The name of the station to get.</param>
    /// <returns></returns>
    public VanillaStation? GetVanillaStationByName(string stationName)
    {
        return VanillaStations.FirstOrDefault(s =>
            string.Equals(s.StationName, stationName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Cancels the currently running operation.
    /// </summary>
    public void CancelOperation()
    {
        _cancellationTokenSource?.Cancel();
    }

    #region Properties

    /// <summary>
    ///     Gets the working directory where temporary files are stored during operations. Defaults to
    ///     <c>%LOCALAPPDATA%\RadioExt-Helper\tools\audio</c>.
    /// </summary>
    public string? WorkingDirectory { get; private set; }

    /// <summary>
    ///     Gets the temporary directory for the WolvenKit files. Set to the same directory as
    ///     <see cref="AudioManager.WolvenKitTempDirectory" />.
    /// </summary>
    public string? WolvenKitTempDirectory { get; private set; }

    /// <summary>
    ///     Gets the path to the audio import directory. Defaults to <c>%LOCALAPPDATA%\RadioExt-Helper\tools\audio\imported</c>
    ///     .
    ///     <para>This directory is where audio files that have been imported are stored.</para>
    /// </summary>
    public string? AudioImportDirectory { get; private set; }

    /// <summary>
    ///     Gets the path to the imported working directory. Defaults to
    ///     <c>%LOCALAPPDATA%\RadioExt-Helper\tools\audio\imported_working_directory</c>.
    /// </summary>
    public string? ImportedWorkingDirectory { get; private set; }

    /// <summary>
    ///     The URL to the Google Sheets document containing the vanilla radio station data.
    /// </summary>
    public string RadioStationsSheetURL { get; } =
        "https://docs.google.com/spreadsheets/d/1N9f2i7cBvU4LNDBu57JqFOj1kqWIHX7HKS-BYIZiCKk/edit?gid=1338899553#gid=1338899553";

    /// <summary>
    /// Gets the collection of vanilla stations.
    /// </summary>
    public List<VanillaStation> VanillaStations { get; private set; } = [];

    /// <summary>
    ///     Gets a value indicating whether the audio manager has been initialized.
    /// </summary>
    public bool IsInitialized { get; private set; }

    #endregion
}