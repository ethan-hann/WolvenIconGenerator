using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIG.Lib.Tools.InkAtlas;

internal class Header
{
    public string WolvenKitVersion { get; set; } = "8.13.0-nightly.2024-03-17";
    public string WKitJsonVersion { get; set; } = "0.0.8";
    public int GameVersion { get; set; } = 2120;
    public string ExportedDateTime { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ");
    public string DataType { get; set; } = "CR2W";
    public string ArchiveFileName { get; set; } = string.Empty;
}