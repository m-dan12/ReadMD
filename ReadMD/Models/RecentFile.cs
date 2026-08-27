using System;

namespace ReadMD.Models;

public class RecentFile
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string PreviewText { get; set; } = string.Empty;
    public DateTime LastOpenedAt { get; set; }
}
