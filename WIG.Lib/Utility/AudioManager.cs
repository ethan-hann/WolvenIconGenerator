// AudioManager.cs : WIG.Lib
// Copyright (C) 2025  Ethan Hann
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

#region

using AetherUtils.Core.Files;
using AetherUtils.Core.Logging;
using WIG.Lib.Models.Audio;
using WIG.Lib.Services;

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

    private Cli? _wolvenKitCli;
    private string _wolvenKitCliExe = string.Empty;

    private Cli? _wwiseCli;
    private string _sound2WemScriptPath = string.Empty;
    private string _ffmpegExe = string.Empty;
    private string _wwiseCliExe = string.Empty;

    private bool _isSound2WemDownloaded;
    private bool _isSound2WemExtracted;
    private bool _isWwiseDownloaded;
    private bool _isWwiseExtracted;

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

    /// <summary>
    /// Clean up resources used by the AudioManager.
    /// </summary>
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
            if (!IconManager.Instance.IsInitialized)
                throw new InvalidOperationException("IconManager must be initialized before AudioManager.");

            SetupRequiredPaths();

            if (WorkingDirectory == null)
                throw new InvalidOperationException("The working directory is null.");

            // Fetch and save the vanilla stations JSON file
            var jsonOutputPath = Path.Combine(WorkingDirectory, "vanillaStations.json");
            var fetcher = new FetchVanillaStations(RadioStationsSheetUrl, jsonOutputPath);

            VanillaStations = await fetcher.FetchAndSaveAsync();

            if (WolvenKitTempDirectory == null)
                throw new InvalidOperationException("The WolvenKit temp directory is null.");

            // Setup WolvenKit CLI
            _wolvenKitCliExe = Path.Combine(WolvenKitTempDirectory, "WolvenKit.CLI.exe");

            if (!File.Exists(_wolvenKitCliExe))
                throw new FileNotFoundException("The WolvenKit CLI executable could not be found.", _wolvenKitCliExe);

            _wolvenKitCli = new Cli(_wolvenKitCliExe, _cancellationTokenSource!.Token);

            await DownloadAudioToolsIfRequiredAsync();

            if (Sound2WemDirectory == null)
                throw new InvalidOperationException("The sound2wem directory is null.");

            if (WwiseToolsDirectory == null)
                throw new InvalidOperationException("The Wwise tools directory is null.");

            _sound2WemScriptPath = ResolveSound2WemScriptPath();

            _wwiseCliExe = ResolveWwiseConsolePath();

            _ffmpegExe = ResolveFfmpegPath();

            if (!File.Exists(_sound2WemScriptPath))
                throw new FileNotFoundException("The sound2wem script could not be found.", _sound2WemScriptPath);

            if (!File.Exists(_wwiseCliExe))
                throw new FileNotFoundException("The WwiseConsole executable could not be found.", _wwiseCliExe);

            if (string.IsNullOrWhiteSpace(_ffmpegExe) || !File.Exists(_ffmpegExe))
                throw new FileNotFoundException("The FFmpeg executable could not be found.", _ffmpegExe);

            _wwiseCli = new Cli(_sound2WemScriptPath, _cancellationTokenSource.Token, Sound2WemDirectory);

            //Subscribe to events
            _wolvenKitCli.OutputChanged += (_, output) => OnCliProgressChanged(output);
            _wolvenKitCli.ErrorChanged += (_, error) => OnCliErrorOccurred(error);
            _wwiseCli.OutputChanged += (_, output) => OnCliProgressChanged(output);
            _wwiseCli.ErrorChanged += (_, error) => OnCliErrorOccurred(error);

            IsInitialized = VanillaStations.Count != 0 && _wolvenKitCli != null && _wwiseCli != null;
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
                "Wolven Icon Generator", "tools", "audio");
            AudioImportDirectory = Path.Combine(WorkingDirectory, "imported");
            ImportedWorkingDirectory = Path.Combine(WorkingDirectory, "imported_working_directory");
            Sound2WemDirectory = Path.Combine(WorkingDirectory, "sound2wem");
            WwiseToolsDirectory = Path.Combine(WorkingDirectory, "wwise");
            FfmpegDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RadioExt-Helper", "ffmpeg");
            WolvenKitTempDirectory = IconManager.Instance.WolvenKitTempDirectory;

            Directory.CreateDirectory(WorkingDirectory);
            Directory.CreateDirectory(AudioImportDirectory);
            Directory.CreateDirectory(ImportedWorkingDirectory);
            Directory.CreateDirectory(Sound2WemDirectory);
            Directory.CreateDirectory(WwiseToolsDirectory);
            //No need to create WolvenKitTempDirectory, IconManager should have already created it.

            AuLogger.GetCurrentLogger<AudioManager>("SetupRequiredPaths").Info($"WorkingDirectory: {WorkingDirectory}");
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<AudioManager>("SetupRequiredPaths").Error(ex);
        }
    }

    #region Audio Tool Downloads

    private async Task DownloadAudioToolsIfRequiredAsync()
    {
        await DownloadSound2WemIfRequiredAsync();
        await DownloadWwiseIfRequiredAsync();
    }

    private async Task DownloadSound2WemIfRequiredAsync()
    {
        if (Sound2WemDirectory == null || WorkingDirectory == null)
            throw new InvalidOperationException("The sound2wem or working directory is null.");

        if (File.Exists(Path.Combine(Sound2WemDirectory, "zSound2wem.cmd")) ||
            File.Exists(Path.Combine(Sound2WemDirectory, "sound2wem.cmd")))
        {
            _isSound2WemDownloaded = true;
            _isSound2WemExtracted = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(Sound2WemDownloadUrl))
            throw new InvalidOperationException("The sound2wem download URL is null or empty.");

        var zipFile = Path.Combine(WorkingDirectory, "sound2wem.zip");
        await PathHelper.DownloadFileAsync(Sound2WemDownloadUrl, zipFile);
        _isSound2WemDownloaded = File.Exists(zipFile);

        if (!_isSound2WemDownloaded)
            throw new InvalidOperationException("The sound2wem files could not be downloaded.");

        var tempExtractPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempExtractPath);

        try
        {
            await PathHelper.ExtractZipFileAsync(zipFile, tempExtractPath);

            var extractedRoot = GetExtractedRootDirectory(tempExtractPath);
            CopyDirectoryContents(extractedRoot, Sound2WemDirectory, true);

            _isSound2WemExtracted =
                File.Exists(Path.Combine(Sound2WemDirectory, "zSound2wem.cmd")) ||
                File.Exists(Path.Combine(Sound2WemDirectory, "sound2wem.cmd"));

            if (!_isSound2WemExtracted)
                throw new FileNotFoundException("The sound2wem script file could not be extracted.");
        }
        finally
        {
            if (Directory.Exists(tempExtractPath))
                Directory.Delete(tempExtractPath, true);
        }
    }

    private async Task DownloadWwiseIfRequiredAsync()
    {
        if (WwiseToolsDirectory == null || WorkingDirectory == null)
            throw new InvalidOperationException("The Wwise tools or working directory is null.");

        var existingWwisePath = ResolveWwiseConsolePath(false);
        if (!string.IsNullOrWhiteSpace(existingWwisePath) && File.Exists(existingWwisePath))
        {
            _isWwiseDownloaded = true;
            _isWwiseExtracted = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(WwiseDownloadUrl))
            throw new InvalidOperationException("The Wwise download URL is null or empty.");

        var zipFile = Path.Combine(WorkingDirectory, "wwise.zip");
        await PathHelper.DownloadFileAsync(WwiseDownloadUrl, zipFile);
        _isWwiseDownloaded = File.Exists(zipFile);

        if (!_isWwiseDownloaded)
            throw new InvalidOperationException("The Wwise tools could not be downloaded.");

        var tempExtractPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempExtractPath);

        try
        {
            await PathHelper.ExtractZipFileAsync(zipFile, tempExtractPath);

            var extractedRoot = GetExtractedRootDirectory(tempExtractPath);
            CopyDirectoryContents(extractedRoot, WwiseToolsDirectory, true);

            _isWwiseExtracted = !string.IsNullOrWhiteSpace(ResolveWwiseConsolePath(false));
            if (!_isWwiseExtracted)
                throw new FileNotFoundException("WwiseConsole.exe could not be found after extraction.");
        }
        finally
        {
            if (Directory.Exists(tempExtractPath))
                Directory.Delete(tempExtractPath, true);
        }
    }

    private string ResolveSound2WemScriptPath()
    {
        if (!string.IsNullOrWhiteSpace(Sound2WemScriptPath))
            return Sound2WemScriptPath;

        if (Sound2WemDirectory == null)
            return string.Empty;

        var zScript = Path.Combine(Sound2WemDirectory, "zSound2wem.cmd");
        if (File.Exists(zScript))
            return zScript;

        var script = Path.Combine(Sound2WemDirectory, "sound2wem.cmd");
        return File.Exists(script) ? script : zScript;
    }

    private string ResolveWwiseConsolePath(bool useConfiguredPath = true)
    {
        if (useConfiguredPath && !string.IsNullOrWhiteSpace(WwiseConsoleExecutablePath))
            return WwiseConsoleExecutablePath;

        if (WwiseToolsDirectory == null)
            return string.Empty;

        var candidates = new[]
        {
            Path.Combine(WwiseToolsDirectory, "Authoring", "x64", "Release", "bin", "WwiseConsole.exe"),
            Path.Combine(WwiseToolsDirectory, "x64", "Release", "bin", "WwiseConsole.exe")
        };

        return candidates.FirstOrDefault(File.Exists) ?? candidates[0];
    }

    private string ResolveFfmpegPath()
    {
        if (!string.IsNullOrWhiteSpace(FfmpegExecutablePath))
        {
            if (File.Exists(FfmpegExecutablePath))
                return FfmpegExecutablePath;

            if (Directory.Exists(FfmpegExecutablePath))
            {
                var configuredCandidates = new[]
                {
                    Path.Combine(FfmpegExecutablePath, "ffmpeg.exe"),
                    Path.Combine(FfmpegExecutablePath, "bin", "ffmpeg.exe")
                };

                var configuredCandidate = configuredCandidates.FirstOrDefault(File.Exists);
                if (!string.IsNullOrWhiteSpace(configuredCandidate))
                    return configuredCandidate;
            }
        }

        if (string.IsNullOrWhiteSpace(FfmpegDirectory) || !Directory.Exists(FfmpegDirectory))
            return FfmpegExecutablePath;

        var candidates = new[]
        {
            Path.Combine(FfmpegDirectory, "ffmpeg.exe"),
            Path.Combine(FfmpegDirectory, "bin", "ffmpeg.exe")
        };

        var candidate = candidates.FirstOrDefault(File.Exists);
        if (!string.IsNullOrWhiteSpace(candidate))
            return candidate;

        var discovered = Directory.EnumerateFiles(FfmpegDirectory, "ffmpeg.exe", SearchOption.AllDirectories)
            .FirstOrDefault();

        return discovered ?? FfmpegExecutablePath;
    }

    private static string GetExtractedRootDirectory(string extractedDirectory)
    {
        var directories = Directory.GetDirectories(extractedDirectory);
        return directories.Length == 1 ? directories[0] : extractedDirectory;
    }

    private static void CopyDirectoryContents(string sourceDirectory, string destinationDirectory, bool overwrite)
    {
        Directory.CreateDirectory(destinationDirectory);

        foreach (var file in Directory.GetFiles(sourceDirectory))
        {
            var destinationFilePath = Path.Combine(destinationDirectory, Path.GetFileName(file));
            File.Copy(file, destinationFilePath, overwrite);
        }

        foreach (var directory in Directory.GetDirectories(sourceDirectory))
        {
            var destinationPath = Path.Combine(destinationDirectory, Path.GetFileName(directory));
            CopyDirectoryContents(directory, destinationPath, overwrite);
        }
    }

    #endregion

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

    /// <summary>
    /// Releases unmanaged resources and performs cleanup operations before the AudioManager object is reclaimed by
    /// garbage collection.
    /// </summary>
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

            // Create the path that imported audio files are stored
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

            // Create the base path for the REDEngine archive files
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
    /// Asynchronously generates an <c>.archive</c> file from a list of audio files and a vanilla station name.
    /// Renames the audio files to match the WEM IDs from the vanilla station that the list of <see cref="ReplacementTrack"/>s are replacing.
    /// This method supports progress reporting and task cancellation.
    /// </summary>
    /// <param name="vanillaStationName">The original, vanilla station to retrieve filenames from.</param>
    /// <param name="files">The list of files to use as replacement tracks.</param>
    /// <param name="progress">An optional progress reporter to track the progress of the operation.</param>
    /// <param name="overwrite">Indicates whether existing directories should be overwritten if they exist.</param>
    /// <param name="cancellationToken">The cancellation token for stopping the task.</param>
    /// <returns>A task that, when complete, returns a <see cref="WolvenAudio"/> or <c>null</c> if the operation is canceled or fails.</returns>
    /// <exception cref="InvalidOperationException">Occurs when the audio manager has not been initialized.</exception>
    public async Task<WolvenAudio?> GenerateAudioArchiveAsync(string vanillaStationName, List<ReplacementTrack> files,
        IProgress<int>? progress = null, bool overwrite = true, CancellationToken cancellationToken = default)
    {
        _currentProgress = 0;
        if (!IsInitialized)
            throw new InvalidOperationException("The audio manager has not been initialized.");

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = _cancellationTokenSource.Token;

        try
        {
            AuLogger.GetCurrentLogger<AudioManager>("GenerateAudioArchiveAsync")
                .Info($"The audio import operation has started: {vanillaStationName}");
            OnAudioImportStarted(new StatusEventArgs($"The audio import operation has started: {vanillaStationName}",
                false,
                _currentProgress));

            WolvenAudio? audio = null;

            // Task.Run allows async cancellation support inside the method
            await Task.Run(async () =>
            {
                audio = await CreateAudioArchiveAsync(vanillaStationName, files, overwrite, token, progress);
                if (audio == null)
                    throw new InvalidOperationException("The audio archive could not be created.");
            }, token);

            AuLogger.GetCurrentLogger<AudioManager>("GenerateAudioArchiveAsync")
                .Info($"The audio import operation has completed successfully: {audio?.VanillaStation?.StationName}");
            OnAudioImportFinished(new StatusEventArgs(
                $"The audio import operation has completed successfully: {audio?.VanillaStation?.StationName}", false,
                _currentProgress));

            return audio;
        }
        catch (OperationCanceledException)
        {
            AuLogger.GetCurrentLogger<AudioManager>("GenerateAudioArchiveAsync")
                .Info($"The audio import operation was cancelled: {vanillaStationName}");
            OnAudioImportStatus(new StatusEventArgs($"The audio import operation was cancelled: {vanillaStationName}",
                false,
                _currentProgress));
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<AudioManager>("GenerateAudioArchiveAsync").Error(e, e.Message);
            OnAudioImportStatus(new StatusEventArgs(e.Message, true, _currentProgress));
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
    /// Asynchronously creates a <see cref="WolvenAudio"/> object from the specified vanilla station name and list of replacement tracks.
    /// This method supports cancellation and progress reporting.
    /// </summary>
    /// <param name="vanillaStationName">The name of the vanilla station we are replacing.</param>
    /// <param name="files">A list of <see cref="ReplacementTrack"/> that specify the tracks to replace in the vanilla station.</param>
    /// <param name="overwrite">Indicates whether existing files should be overwritten.</param>
    /// <param name="token">The token used for cancelling operations.</param>
    /// <param name="progress">An optional progress reporter for reporting progress.</param>
    /// <returns>A task containing the resulting <see cref="WolvenAudio"/>.</returns>
    private async Task<WolvenAudio> CreateAudioArchiveAsync(string vanillaStationName, List<ReplacementTrack> files,
        bool overwrite,
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
                OnAudioImportStarted(new StatusEventArgs(
                    $"The audio file could not be found: {file.ReplacementFilePath}", true, _currentProgress));
                CancelOperation();
                throw new FileNotFoundException("The audio file could not be found.", file.ReplacementFilePath);
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
                if (!Path.GetExtension(file.ReplacementFilePath).Equals(".wav", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException(
                        $"Only .wav files are supported for WEM conversion. Invalid file: {file.ReplacementFilePath}");

                var stagedWavPath = Path.Combine(projectDirectories["importedAudio"], $"{file.WemId}.wav");
                File.Copy(file.ReplacementFilePath, stagedWavPath, overwrite);
            }

            _currentProgress += 10;
            progress?.Report(_currentProgress);
            token.ThrowIfCancellationRequested();

            // Convert staged WAV files into WEM files directly in source/archive.
            await _wwiseCli!.ConvertWavToWemAsync(_ffmpegExe, _wwiseCliExe,
                projectDirectories["archiveBasePath"], projectDirectories["importedAudio"], token,
                progress, WwiseConversionProfile);

            _currentProgress += 10;
            progress?.Report(_currentProgress);
            token.ThrowIfCancellationRequested();

            // Ensure converted WEM files exist in source/archive.
            foreach (var file in files)
            {
                var convertedWemPath = Path.Combine(projectDirectories["archiveBasePath"], $"{file.WemId}.wem");
                if (!File.Exists(convertedWemPath))
                    throw new FileNotFoundException("The converted WEM file could not be found.", convertedWemPath);
            }

            _currentProgress += 10;
            progress?.Report(_currentProgress);
            token.ThrowIfCancellationRequested();

            _currentProgress += 20;
            progress?.Report(_currentProgress);
            token.ThrowIfCancellationRequested();

            // Pack the project into a .archive file
            await _wolvenKitCli!.PackArchiveAsync(projectDirectories["archiveBasePath"],
                projectDirectories["projectBasePath"], token, progress);

            // Rename the .archive file to the station name
            var originalArchivePath = Path.Combine(projectDirectories["projectBasePath"], "archive.archive");
            var newArchivePath = Path.Combine(projectDirectories["projectBasePath"], $"{vanillaStationName}.archive");
            File.Move(originalArchivePath, newArchivePath, overwrite);

            _currentProgress += 10;
            progress?.Report(_currentProgress);
            token.ThrowIfCancellationRequested();

            // Verify the new archive exists
            if (!File.Exists(newArchivePath))
            {
                OnAudioImportStatus(new StatusEventArgs("The final .archive file could not be renamed.", true,
                    _currentProgress));
                CancelOperation();
                throw new FileNotFoundException("The final .archive file could not be renamed.", newArchivePath);
            }

            _currentProgress += 5;
            progress?.Report(_currentProgress);
            token.ThrowIfCancellationRequested();

            // Delete the original archive
            File.Delete(originalArchivePath);

            //Create and return the WolvenAudio object
            var audio = new WolvenAudio(vanillaStationName, newArchivePath, files)
            {
                OriginalArchivePath = originalArchivePath,
                Sha256HashOfArchiveFile = HashUtils.ComputeSha256Hash(newArchivePath, true),
                AudioId = Guid.Parse(projectDirectories["replacedStationGuid"])
            };

            _currentProgress = 100;
            progress?.Report(_currentProgress);
            return audio;
        }
        catch (Exception e)
        {
            AuLogger.GetCurrentLogger<AudioManager>("CreateAudioArchiveAsync").Error(e, e.Message);
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

    #region Properties

    /// <summary>
    ///     Gets the working directory where temporary files are stored during operations. Defaults to
    ///     <c>%LOCALAPPDATA%\RadioExt-Helper\tools\audio</c>.
    /// </summary>
    public string? WorkingDirectory { get; private set; }

    /// <summary>
    /// Gets the directory path that stores <c>sound2wem</c> files. Defaults to <c>%APPDATA%\Wolven Icon Generator\tools\audio\sound2wem</c>.
    /// </summary>
    public string? Sound2WemDirectory { get; private set; }

    /// <summary>
    /// Gets the directory path that stores Wwise authoring tools. Defaults to <c>%APPDATA%\Wolven Icon Generator\tools\audio\wwise</c>.
    /// </summary>
    public string? WwiseToolsDirectory { get; private set; }

    /// <summary>
    ///     Gets the temporary directory for the WolvenKit files. Set to the same directory as
    ///     <see cref="IconManager.WolvenKitTempDirectory" />.
    /// </summary>
    public string? WolvenKitTempDirectory { get; private set; }

    /// <summary>
    /// Gets or sets the path to the FFmpeg executable used for WAV to WEM conversion.
    /// You may also provide a directory path and the executable will be resolved from it.
    /// </summary>
    public string FfmpegExecutablePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets the default FFmpeg install directory from RadioExt-Helper.
    /// </summary>
    public string? FfmpegDirectory { get; private set; }

    /// <summary>
    /// Gets or sets the download URL for the Wwise authoring tools archive.
    /// </summary>
    public string WwiseDownloadUrl { get; set; } = "https://tortal.xyz/TEx4C";

    /// <summary>
    /// Gets or sets the download URL for the sound2wem archive.
    /// </summary>
    public string Sound2WemDownloadUrl { get; set; } =
        "https://github.com/EternalLeo/sound2wem/archive/refs/heads/master.zip";

    /// <summary>
    /// Gets or sets the path to the sound2wem script.
    /// </summary>
    public string? Sound2WemScriptPath { get; set; }

    /// <summary>
    /// Gets or sets the path to the WwiseConsole executable.
    /// </summary>
    public string? WwiseConsoleExecutablePath { get; set; }

    /// <summary>
    /// Gets or sets the Wwise conversion profile passed to sound2wem.
    /// </summary>
    public string WwiseConversionProfile { get; set; } = "Vorbis Quality High";

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
    public static string RadioStationsSheetUrl =>
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