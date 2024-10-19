// HashUtils.cs : WIG.Lib
// Copyright (C) 2024  Ethan Hann
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

using System.Security.Cryptography;

namespace WIG.Lib.Utility;

public sealed class HashUtils
{
    /// <summary>
    /// Calculates the SHA-256 hash of the file at the specified path.
    /// </summary>
    /// <param name="file">The file to calculate the hash of.</param>
    /// <param name="useLowerInvariant">Indicates whether the hash should be converted to lower-invariant case.</param>
    /// <returns>The SHA-256 hash of the file</returns>
    public static string ComputeSha256Hash(string file, bool useLowerInvariant)
    {
        // Calculate the hash of the icon file
        using var sha256 = SHA256.Create();
        using var fileStream = File.OpenRead(file);

        var hashBytes = sha256.ComputeHash(fileStream);
        var fileHash = BitConverter.ToString(hashBytes).Replace("-", "");
        fileHash = useLowerInvariant ? fileHash.ToLowerInvariant() : fileHash.ToUpperInvariant();
        return fileHash;
    }

    /// <summary>
    /// Compares two SHA256 hashes for equality.
    /// </summary>
    /// <param name="hash1">The first hash.</param>
    /// <param name="hash2">The second hash.</param>
    /// <returns><c>true</c> if the two hashes are equal; <c>false</c> otherwise.</returns>
    public static bool CompareSha256Hash(string hash1, string hash2)
    {
        return string.Equals(hash1, hash2, StringComparison.OrdinalIgnoreCase);
    }
}