using System;
using System.Threading.Tasks;
using ReadMD.Models;

namespace ReadMD.Services;

public interface IReadingSettingsService
{
    LineWidth LineWidth { get; set; }
    bool UseSerifs { get; set; }
    int FontSize { get; set; }
    LineSpacing LineSpacing { get; set; }

    event Action? SettingsChanged;

    Task LoadSettingsAsync();
    Task SaveSettingsAsync();
}

public class ReadingSettingsService : IReadingSettingsService
{
    private readonly SettingsStorageService _storageService;

    private LineWidth _lineWidth = LineWidth.Medium;
    private bool _useSerifs = true;
    private int _fontSize = 16;
    private LineSpacing _lineSpacing = LineSpacing.Normal;

    public ReadingSettingsService(SettingsStorageService storageService)
    {
        _storageService = storageService;
    }

    public LineWidth LineWidth { get => _lineWidth; set => SetAndNotify(ref _lineWidth, value); }
    public bool UseSerifs { get => _useSerifs; set => SetAndNotify(ref _useSerifs, value); }
    public int FontSize { get => _fontSize; set => SetAndNotify(ref _fontSize, value); }
    public LineSpacing LineSpacing { get => _lineSpacing; set => SetAndNotify(ref _lineSpacing, value); }

    public event Action? SettingsChanged;

    public async Task LoadSettingsAsync()
    {
        var settings = await _storageService.LoadSettingsAsync();
        if (settings != null)
        {
            _useSerifs = settings.FontFamily == "Serif";
            _fontSize = (int)settings.FontSize;
            _lineSpacing = Enum.Parse<LineSpacing>(settings.LineSpacing);
            _lineWidth = Enum.Parse<LineWidth>(settings.LineWidth);
            SettingsChanged?.Invoke();
        }
    }

    public async Task SaveSettingsAsync()
    {
        var settings = new AppSettings
        {
            FontFamily = UseSerifs ? "Serif" : "Sans",
            FontSize = FontSize,
            LineSpacing = LineSpacing.ToString(),
            LineWidth = LineWidth.ToString()
        };

        await _storageService.SaveSettingsAsync(settings);
    }

    private void SetAndNotify<T>(ref T field, T value)
    {
        if (Equals(field, value)) return;
        field = value;
        SettingsChanged?.Invoke();

        // Автосохранение при изменении настроек
        _ = SaveSettingsAsync();
    }
}

public enum LineWidth { Narrow, Medium, Wide }
public enum LineSpacing { Compact, Normal, Relaxed }