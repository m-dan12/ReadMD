using System;
using System.Collections.Generic;
using System.Text;

namespace ReadMD.Services;

// Services/IReadingSettingsService.cs
public interface IReadingSettingsService
{
    LineWidth LineWidth { get; set; }
    event Action? SettingsChanged;
}

// Services/ReadingSettingsService.cs
public class ReadingSettingsService : IReadingSettingsService
{
    private LineWidth _lineWidth = LineWidth.Medium;

    public LineWidth LineWidth
    {
        get => _lineWidth;
        set
        {
            if (_lineWidth == value) return;
            _lineWidth = value;
            SettingsChanged?.Invoke();
        }
    }

    public event Action? SettingsChanged;
}

public enum LineWidth
{
    Narrow,
    Medium,
    Wide
}