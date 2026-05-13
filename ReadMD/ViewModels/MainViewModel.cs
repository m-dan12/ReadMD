using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using ReadMD.Services;

namespace ReadMD.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IReadingSettingsService _readingSettings;

    [ObservableProperty]
    private double lineWidthWidth = 600;

    [ObservableProperty]
    private double lineWidthMaxWidth = 800;

    public MainViewModel(IReadingSettingsService readingSettings)
    {
        _readingSettings = readingSettings;
        UpdateWidths(readingSettings.LineWidth);

        // Подписка на изменения
        _readingSettings.SettingsChanged += () =>
        {
            UpdateWidths(_readingSettings.LineWidth);
        };
    }

    private void UpdateWidths(LineWidth width)
    {
        (LineWidthWidth, LineWidthMaxWidth) = width switch
        {
            LineWidth.Narrow => (500, 600),
            LineWidth.Medium => (700, 850),
            LineWidth.Wide => (900, 1100),
            _ => (700, 850)
        };
    }
}