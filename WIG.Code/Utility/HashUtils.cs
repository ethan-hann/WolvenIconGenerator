using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WolvenIconGenerator.Utility
{
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
    }
}
