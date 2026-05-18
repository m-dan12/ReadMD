using System;

namespace ReadMD.Services;

public interface IReadingSettingsService
{
    LineWidth LineWidth { get; set; }
    bool UseSerifs { get; set; }
    int FontSize { get; set; }
    LineSpacing LineSpacing { get; set; }

    event Action? SettingsChanged;
}

public class ReadingSettingsService : IReadingSettingsService
{
    private LineWidth _lineWidth = LineWidth.Medium;
    private bool _useSerifs = true;
    private int _fontSize = 16;
    private LineSpacing _lineSpacing = LineSpacing.Normal;

    public LineWidth LineWidth { get => _lineWidth; set => SetAndNotify(ref _lineWidth, value); }
    public bool UseSerifs { get => _useSerifs; set => SetAndNotify(ref _useSerifs, value); }
    public int FontSize { get => _fontSize; set => SetAndNotify(ref _fontSize, value); }
    public LineSpacing LineSpacing { get => _lineSpacing; set => SetAndNotify(ref _lineSpacing, value); }

    public event Action? SettingsChanged;

    private void SetAndNotify<T>(ref T field, T value)
    {
        if (Equals(field, value)) return;
        field = value;
        SettingsChanged?.Invoke();
    }
}

public enum LineWidth { Narrow, Medium, Wide }
public enum LineSpacing { Compact, Normal, Relaxed }