using AetherUtils.Core.Files;
using AetherUtils.Core.Logging;
using System.Diagnostics;
using System.IO.Pipes;
using WIG.Lib.Models;

namespace WolvenIconGenerator.Utility
{
    /// <summary>
    /// Integration class used for communication with the Cyber Radio Assistant (CRA) application.
    /// </summary>
    public sealed class CraIntegration
    {
        private static readonly Json<WolvenIcon> JsonSerializer = new();

        /// <summary>
        /// Send the specified icon to the Cyber Radio Assistant (CRA) application. If CRA is not running, the icon will not be sent and a warning will be logged.
        /// </summary>
        /// <param name="icon">The icon to send.</param>
        public static void SendIconToCra(WolvenIcon icon)
        {
            if (!IsCraRunning())
            {
                AuLogger.GetCurrentLogger<CraIntegration>().Warn("CRA is not running. Icon will not be sent.");
                return;
            }

            var json = JsonSerializer.ToJson(icon);

            using NamedPipeClientStream pipeClient = new(".", "CRANamedPipe", PipeDirection.Out);
            pipeClient.Connect(TimeSpan.FromSeconds(5));

            using var writer = new StreamWriter(pipeClient);
            writer.Write(json);
            writer.Flush();
        }

        public static bool IsCraRunning()
        {
            return IsProcessRunning("RadioExt-Helper");
        }

        private static bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }
    }
}
