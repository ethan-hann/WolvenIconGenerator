// FetchVanillaStations.cs : WIG.Lib
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

using System.Globalization;
using System.Text;
using AetherUtils.Core.Logging;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using WIG.Lib.Models.Audio;
using WIG.Lib.Properties;

#endregion

namespace WIG.Lib.Services;

/// <summary>
///     Service that loads vanilla station data from the snapshot embedded in WIG.Lib and
///     generates a JSON file for runtime use. The live Google Sheet is only used to check
///     whether the snapshot is out of date.
/// </summary>
/// <remarks>
///     The snapshot is <c>Resources/cyberpunk2077_vanillaStations.csv</c>. To refresh it, export the sheet as CSV,
///     verify the WEM IDs, replace the file, and publish a new WIG.Lib version.
/// </remarks>
public class FetchVanillaStations
{
    private readonly string _gid;
    private readonly string _outputPath;
    private readonly string _sheetId;

    /// <summary>
    ///     Create a new instance of the FetchVanillaStations service.
    /// </summary>
    /// <param name="sheetEditUrl">The Google sheets public edit URL.</param>
    /// <param name="outputPath">The path to save the <c>vanillaStations.json</c> file.</param>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="sheetEditUrl" /> or <paramref name="outputPath" /> is
    ///     <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidOperationException">If the sheet ID in <paramref name="sheetEditUrl" /> is invalid.</exception>
    public FetchVanillaStations(string sheetEditUrl, string outputPath)
    {
        if (string.IsNullOrWhiteSpace(sheetEditUrl))
            throw new ArgumentNullException(nameof(sheetEditUrl));

        _outputPath = outputPath ?? throw new ArgumentNullException(nameof(outputPath));

        // Extract Sheet ID and GID from the edit URL
        var parts = sheetEditUrl.Split(['/'], StringSplitOptions.RemoveEmptyEntries);
        _sheetId = parts.FirstOrDefault(p => p.Length == 44) ??
                   throw new InvalidOperationException("Invalid Sheet ID in URL");

        var gidParam = sheetEditUrl.Split(["gid="], StringSplitOptions.None).LastOrDefault();
        _gid = gidParam ?? "0";
    }

    private string BuildCsvUrl()
    {
        return $"https://docs.google.com/spreadsheets/d/{_sheetId}/export?format=csv&gid={_gid}";
    }

    /// <summary>
    ///     Loads the vanilla station snapshot embedded in WIG.Lib, parses it into objects, and writes vanillaStations.json.
    ///     Returns a list of stations for further processing and lookup.
    /// </summary>
    /// <remarks>
    ///     This doesn't contact Google Sheets, so it works offline. Use <see cref="IsSnapshotOutdatedAsync" /> to check
    ///     whether the live sheet has changed since the snapshot was taken.
    /// </remarks>
    public async Task<List<VanillaStation>> FetchAndSaveAsync()
    {
        var log = AuLogger.GetCurrentLogger<FetchVanillaStations>();

        try
        {
            // Load from the verified snapshot so a sheet edit can't silently change WEM IDs for users
            log.Info("Loading vanilla station data from the embedded snapshot.");
            var stationList = await ParseStationsAsync(GetSnapshotCsv());

            var json = JsonConvert.SerializeObject(stationList, Formatting.Indented);

            var directory = Path.GetDirectoryName(_outputPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllTextAsync(_outputPath, json).ConfigureAwait(false);

            log.Info($"Loaded {stationList.Count} stations. JSON saved to {_outputPath}");

            return stationList;
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<FetchVanillaStations>()
                .Error(ex, "Error loading or processing vanilla station data.");
            throw;
        }
    }

    /// <summary>
    ///     Fetches the live Google Sheet and compares its station data to the embedded snapshot.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the sheet's station data differs from the snapshot; <c>false</c> if it matches or the sheet
    ///     couldn't be fetched.
    /// </returns>
    public async Task<bool> IsSnapshotOutdatedAsync()
    {
        var log = AuLogger.GetCurrentLogger<FetchVanillaStations>();

        try
        {
            var csvUrl = BuildCsvUrl();
            log.Info($"Checking the vanilla station snapshot against: {csvUrl}");

            using var httpClient = new HttpClient();
            var liveCsv = await httpClient.GetStringAsync(csvUrl).ConfigureAwait(false);

            // Compare parsed data, not raw text, so line endings and the sheet's pre-header don't count as changes
            var liveJson = JsonConvert.SerializeObject(await ParseStationsAsync(liveCsv));
            var snapshotJson = JsonConvert.SerializeObject(await ParseStationsAsync(GetSnapshotCsv()));

            var isOutdated = !string.Equals(liveJson, snapshotJson, StringComparison.Ordinal);
            if (isOutdated)
                log.Warn("The vanilla station sheet has changed since this snapshot was taken. Verify the changes and update the snapshot.");
            else
                log.Info("The vanilla station snapshot matches the live sheet.");

            return isOutdated;
        }
        catch (Exception ex)
        {
            log.Error(ex, "Couldn't fetch the vanilla station sheet to check the snapshot.");
            return false;
        }
    }

    /// <summary>
    ///     Gets the embedded vanilla station snapshot as CSV text.
    /// </summary>
    private static string GetSnapshotCsv()
    {
        // Strip a UTF-8 byte order mark if the file was saved with one
        var bytes = Resources.cyberpunk2077_vanillaStations;
        var preamble = Encoding.UTF8.GetPreamble();
        var offset = bytes.AsSpan().StartsWith(preamble) ? preamble.Length : 0;
        return Encoding.UTF8.GetString(bytes, offset, bytes.Length - offset);
    }

    /// <summary>
    ///     Parses the vanilla station sheet's CSV into a list of stations.
    /// </summary>
    private static async Task<List<VanillaStation>> ParseStationsAsync(string csvContent)
    {
        var log = AuLogger.GetCurrentLogger<FetchVanillaStations>();

        // Slice off the Google sheet's pre-header; start at the real header row.
        var normalizedCsv = SliceFromRealHeader(csvContent, out var headerRowIndex);
        if (headerRowIndex < 0)
            log.Warn("Could not locate a real header row; attempting to parse original CSV (may fail).");
        else
            log.Info($"Detected real header at CSV line index {headerRowIndex} (0-based after normalization).");

        var stations = new Dictionary<string, List<AudioTrack>>(StringComparer.Ordinal);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null,
            TrimOptions = TrimOptions.Trim
        };

        var rowCount = 0;

        using (var reader = new StringReader(normalizedCsv))
        using (var csv = new CsvReader(reader, config))
        {
            if (!await csv.ReadAsync() || !csv.ReadHeader())
                throw new InvalidDataException("CSV does not contain a readable header row.");

            // The exact headers we care about
            const string colTrackName = "Track Name";
            const string colArtist = "Artist";
            const string colRadio = "Radio Station";
            const string colDuration = "Duration";
            const string colWemV23 = "WEM File v2.3";

            while (await csv.ReadAsync())
            {
                rowCount++;

                var trackName = csv.GetField(colTrackName);
                var artist = csv.GetField(colArtist);
                var stationName = csv.GetField(colRadio);
                var durationField = csv.GetField(colDuration);
                var wemField = csv.GetField(colWemV23);

                // Skip rows missing essentials
                if (string.IsNullOrWhiteSpace(stationName) || string.IsNullOrWhiteSpace(trackName))
                    continue;

                var durations = ParseDurations(durationField);
                var wemIds = ParseWemIds(wemField);

                var track = new AudioTrack(
                    trackName,
                    artist ?? string.Empty,
                    durations,
                    wemIds
                );

                if (!stations.TryGetValue(stationName, out var list))
                {
                    list = new List<AudioTrack>();
                    stations[stationName] = list;
                }

                list.Add(track);
            }
        }

        var stationList = stations
            .Select(kv => new VanillaStation(kv.Key, kv.Value))
            .ToList();

        log.Info($"Parsed {rowCount} CSV rows into {stationList.Count} stations.");

        return stationList;
    }

    /// <summary>
    ///     Parse duration like "3:35" or "3:35,3:40" into a list of seconds.
    /// </summary>
    private static List<float> ParseDurations(string? durationField)
    {
        var durations = new List<float>();
        if (string.IsNullOrWhiteSpace(durationField))
            return durations;

        var parts = durationField.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var part in parts)
            if (TimeSpan.TryParseExact(part, "m\\:ss", CultureInfo.InvariantCulture, out var ts))
                durations.Add((float)ts.TotalSeconds);
            else if (TimeSpan.TryParse(part, out var ts2) && ts2.TotalSeconds > 0)
                // tolerant fallback if Google changes formatting slightly
                durations.Add((float)ts2.TotalSeconds);

        return durations;
    }

    /// <summary>
    ///     Parse "WEM File v2.3" into a HashSet of clean IDs.
    /// </summary>
    private static HashSet<string> ParseWemIds(string? wemField)
    {
        var wemIds = new HashSet<string>();
        if (string.IsNullOrWhiteSpace(wemField))
            return wemIds;

        // Split on any whitespace; only accept pure-digit tokens
        foreach (var id in wemField.Split((char[])null!,
                     StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            if (id.All(char.IsDigit))
                wemIds.Add(id);

        return wemIds;
    }

    /// <summary>
    ///     Finds the first line that looks like the REAL CSV header row
    ///     (must contain both "Track Name" and "Radio Station") and returns the CSV
    ///     sliced from that line onward. If not found, returns the original content.
    /// </summary>
    private static string SliceFromRealHeader(string csv, out int headerRowIndex)
    {
        // Normalize line endings
        var text = csv.Replace("\r\n", "\n").Replace('\r', '\n');
        var lines = text.Split('\n');

        headerRowIndex = -1;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if (!line.Contains("Track Name", StringComparison.Ordinal) ||
                !line.Contains("Radio Station", StringComparison.Ordinal))
                continue;

            var cols = line.Split(',');
            var hasTrackName = cols.Any(c => c.Trim().Equals("Track Name", StringComparison.Ordinal));
            var hasRadioStation = cols.Any(c => c.Trim().Equals("Radio Station", StringComparison.Ordinal));

            if (!hasTrackName || !hasRadioStation) continue;

            headerRowIndex = i;
            break;
        }

        if (headerRowIndex <= -1)
            // Couldn't find a real header; fall back to original
            return text;

        var sb = new StringBuilder();
        for (var i = headerRowIndex; i < lines.Length; i++)
        {
            sb.Append(lines[i]);
            if (i < lines.Length - 1) sb.Append('\n');
        }

        return sb.ToString();
    }
}