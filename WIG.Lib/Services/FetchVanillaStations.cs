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

#endregion

namespace WIG.Lib.Services;

/// <summary>
///     Service to fetch vanilla station data from a live Google Sheet and
///     generate a JSON file for runtime use.
/// </summary>
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

        // Extract Sheet ID and GID from the edit URL (your original logic).
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
    ///     Fetches the sheet, parses it into objects, and writes vanillaStations.json.
    ///     Returns a list of stations for further processing and lookup.
    /// </summary>
    public async Task<List<VanillaStation>> FetchAndSaveAsync()
    {
        var log = AuLogger.GetCurrentLogger<FetchVanillaStations>();

        try
        {
            var csvUrl = BuildCsvUrl();
            log.Info($"Fetching vanilla station data from: {csvUrl}");

            using var httpClient = new HttpClient();
            string csvContent;

            try
            {
                //Attempt to fetch the CSV from Google Sheets
                csvContent = await httpClient.GetStringAsync(csvUrl).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // If we can't reach Google or something went wrong connecting, try to fall back to existing local copy if it exists
                log.Error(ex, "Couldn't connect to Google to fetch updated sheet. Checking for local copy...");

                // No local copy exists. Can't proceed.
                if (!File.Exists(_outputPath))
                    throw new FileNotFoundException("No local copy of vanillaStations.json exists.", ex);

                log.Info($"Local copy found at {_outputPath}; loading that instead.");
                var existingJson = await File.ReadAllTextAsync(_outputPath).ConfigureAwait(false);
                var existingStations = JsonConvert.DeserializeObject<List<VanillaStation>>(existingJson);

                if (existingStations == null)
                    throw new InvalidDataException("Local copy exists but could not be parsed.");

                log.Info($"Loaded {existingStations.Count} stations from local copy.");
                return existingStations;
            }

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

            var json = JsonConvert.SerializeObject(stationList, Formatting.Indented);

            var directory = Path.GetDirectoryName(_outputPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllTextAsync(_outputPath, json).ConfigureAwait(false);

            log.Info(
                $"Parsed {rowCount} CSV rows. Fetched and processed {stationList.Count} stations. JSON saved to {_outputPath}");

            return stationList;
        }
        catch (Exception ex)
        {
            AuLogger.GetCurrentLogger<FetchVanillaStations>()
                .Error(ex, "Error fetching or processing vanilla station data.");
            throw;
        }
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