using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using ReadMD.Services;

namespace ReadMD.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IReadingSettingsService _readingSettings;
    private readonly IMarkdownDocumentService _markdownDocumentService;

    [ObservableProperty]
    private double lineWidthWidth = 600;

    [ObservableProperty]
    private double lineWidthMaxWidth = 800;

    [ObservableProperty]
    private string markdown = "# Заголовок";

    public MainViewModel(
        IReadingSettingsService readingSettings,
        IMarkdownDocumentService markdownDocumentService)
    {
        _readingSettings = readingSettings;
        _markdownDocumentService = markdownDocumentService;
        markdown = markdownDocumentService.Markdown;

        UpdateWidths(readingSettings.LineWidth);

        // Подписка на изменения
        _readingSettings.SettingsChanged += () =>
        {
            UpdateWidths(_readingSettings.LineWidth);
        };

        _markdownDocumentService.MarkdownChanged += () =>
        {
            Markdown = _markdownDocumentService.Markdown;
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