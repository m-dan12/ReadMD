using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using ExCSS;
using ReadMD.Services;
using System;

namespace ReadMD.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IReadingSettingsService _readingSettings;
    private readonly IMarkdownDocumentService _markdownDocumentService;

    // ── Ширина строки ────────────────────────────────────────────────
    [ObservableProperty] private double lineWidthWidth = 700;
    [ObservableProperty] private double lineWidthMaxWidth = 850;

    // ── Документ ─────────────────────────────────────────────────────
    [ObservableProperty] private string markdown = "# Заголовок";

    // ── Типографика: пробрасываются в биндинги стилей MarkdownViewer ─
    [ObservableProperty] private FontFamily fontFamily = Application.Current!.Resources["LoraFont"] as FontFamily ?? FontFamily.Default;
    [ObservableProperty] private double fontSize = 16;
    [ObservableProperty] private double lineHeight = 24;  // fontSize * lineSpacing

    // Размеры заголовков — масштабируются от базового fontSize
    [ObservableProperty] private double h1Size;
    [ObservableProperty] private double h1LineHeight;
    [ObservableProperty] private double h2Size;
    [ObservableProperty] private double h2LineHeight;
    [ObservableProperty] private double h3Size;
    [ObservableProperty] private double h3LineHeight;
    [ObservableProperty] private double h4Size;
    [ObservableProperty] private double h4LineHeight;
    [ObservableProperty] private double h5Size;
    [ObservableProperty] private double h5LineHeight;
    [ObservableProperty] private double h6Size;
    [ObservableProperty] private double h6LineHeight;

    public MainViewModel(
        IReadingSettingsService readingSettings,
        IMarkdownDocumentService markdownDocumentService)
    {
        _readingSettings = readingSettings;
        _markdownDocumentService = markdownDocumentService;

        markdown = markdownDocumentService.Markdown;

        ApplySettings();

        _readingSettings.SettingsChanged += ApplySettings;
        _markdownDocumentService.MarkdownChanged += () => Markdown = _markdownDocumentService.Markdown;
    }

    private void ApplySettings()
    {
        UpdateWidths(_readingSettings.LineWidth);
        UpdateTypography();
    }

    private void UpdateWidths(LineWidth width)
    {
        (LineWidthWidth, LineWidthMaxWidth) = width switch
        {
            LineWidth.Narrow => (500, 600),
            LineWidth.Medium => (700, 850),
            LineWidth.Wide => (900, 1100),
            _ => (700, 850),
        };
    }

    private void UpdateTypography()
    {
        // Шрифт    
        FontFamily = Application.Current!.Resources[
            _readingSettings.UseSerifs ? "LoraFont" : "LatoFont"
        ] as FontFamily ?? FontFamily.Default;

        // Базовый размер и межстрочный интервал
        var size = (double)_readingSettings.FontSize;
        var spacing = _readingSettings.LineSpacing;

        FontSize = size;
        LineHeight = size * ToDouble(_readingSettings.LineSpacing);

        // Заголовки масштабируются относительно базового размера
        H1Size = Math.Round(size * 2.0);
        H2Size = Math.Round(size * 1.6);
        H3Size = Math.Round(size * 1.4);
        H4Size = Math.Round(size * 1.2);
        H5Size = Math.Round(size * 1.1);
        H6Size = size;
        H1LineHeight = H1Size * ToDouble(_readingSettings.LineSpacing);
        H2LineHeight = H2Size * ToDouble(_readingSettings.LineSpacing);
        H3LineHeight = H3Size * ToDouble(_readingSettings.LineSpacing);
        H4LineHeight = H4Size * ToDouble(_readingSettings.LineSpacing);
        H5LineHeight = H5Size * ToDouble(_readingSettings.LineSpacing);
        H6LineHeight = H6Size * ToDouble(_readingSettings.LineSpacing);
    }
    private static double ToDouble(LineSpacing spacing) => spacing switch
    {
        LineSpacing.Compact => 1,
        LineSpacing.Normal => 1.5,
        LineSpacing.Relaxed => 2,
        _ => 1.5,
    };
}