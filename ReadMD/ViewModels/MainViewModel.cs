using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ReadMD.Services;
using System;
using System.Threading.Tasks;

namespace ReadMD.ViewModels;

public partial class MainViewModel : ViewModelBase, IDisposable
{
    private readonly IReadingSettingsService _readingSettings;
    private readonly IDocumentService _documentService;
    private DispatcherTimer? _autoSaveTimer;

    private const int AutoSaveDelayMs = 2000;

    [ObservableProperty] private double contentWidth = 700;
    [ObservableProperty] private double contentMaxWidth = 850;
    [ObservableProperty] private string markdown = "# ���������";
    [ObservableProperty] private string markdownSource = "# ���������";
    [ObservableProperty] private bool isEditMode;
    [ObservableProperty] private FontFamily fontFamily = Application.Current!.Resources["LoraFont"] as FontFamily ?? FontFamily.Default;
    [ObservableProperty] private double fontSize = 16;
    [ObservableProperty] private double lineHeight = 24;
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
        IDocumentService documentService)
    {
        _readingSettings = readingSettings;
        _documentService = documentService;

        markdown = _documentService.Content;
        markdownSource = markdown;

        ApplySettings();

        _readingSettings.SettingsChanged += ApplySettings;
        _documentService.DocumentChanged += OnDocumentChanged;
    }

    private void OnDocumentChanged(object? sender, EventArgs e)
    {
        Markdown = _documentService.Content;

        if (!IsEditMode)
        {
            MarkdownSource = Markdown;
        }
    }

    private void ApplySettings()
    {
        UpdateWidths(_readingSettings.LineWidth);
        UpdateTypography();
    }

    private void UpdateWidths(LineWidth width) => (ContentWidth, ContentMaxWidth) = width switch
    {
        LineWidth.Narrow => (500, 600),
        LineWidth.Medium => (700, 850),
        LineWidth.Wide => (900, 1100),
        _ => (700, 850),
    };

    private void UpdateTypography()
    {
        FontFamily = Application.Current!.Resources[
            _readingSettings.UseSerifs ? "LoraFont" : "LatoFont"
        ] as FontFamily ?? FontFamily.Default;

        FontSize = _readingSettings.FontSize;
        LineHeight = FontSize * ToDouble(_readingSettings.LineSpacing);

        H1Size = Math.Round(FontSize * 2.0);
        H2Size = Math.Round(FontSize * 1.6);
        H3Size = Math.Round(FontSize * 1.4);
        H4Size = Math.Round(FontSize * 1.2);
        H5Size = Math.Round(FontSize * 1.1);
        H6Size = FontSize;

        H1LineHeight = H1Size * ToDouble(_readingSettings.LineSpacing);
        H2LineHeight = H2Size * ToDouble(_readingSettings.LineSpacing);
        H3LineHeight = H3Size * ToDouble(_readingSettings.LineSpacing);
        H4LineHeight = H4Size * ToDouble(_readingSettings.LineSpacing);
        H5LineHeight = H5Size * ToDouble(_readingSettings.LineSpacing);
        H6LineHeight = H6Size * ToDouble(_readingSettings.LineSpacing);
    }

    partial void OnMarkdownSourceChanged(string value)
    {
        if (IsEditMode)
        {
            Markdown = value;
            RestartAutoSaveTimer();
        }
    }

    partial void OnIsEditModeChanged(bool value)
    {
        if (value)
        {
            MarkdownSource = Markdown;
            return;
        }

        Markdown = MarkdownSource;
        StopAutoSaveTimer();

        if (_documentService.IsLoaded)
        {
            _documentService.Content = MarkdownSource;
            _ = SaveFileAsync();
        }
    }

    private async Task SaveFileAsync()
    {
        try
        {
            await _documentService.SaveAsync();
        }
        catch
        {
            // Игнорируем ошибки сохранения, чтобы UI не ломался.
        }
    }

    private void RestartAutoSaveTimer()
    {
        _autoSaveTimer?.Stop();

        _autoSaveTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(AutoSaveDelayMs)
        };

        _autoSaveTimer.Tick += async (_, _) =>
        {
            StopAutoSaveTimer();

            if (_documentService.IsLoaded)
            {
                _documentService.Content = MarkdownSource;
                await _documentService.SaveAsync();
            }
        };

        _autoSaveTimer.Start();
    }

    private void StopAutoSaveTimer()
    {
        _autoSaveTimer?.Stop();
        _autoSaveTimer = null;
    }

    private static double ToDouble(LineSpacing spacing) => spacing switch
    {
        LineSpacing.Compact => 1.2,
        LineSpacing.Normal => 1.6,
        LineSpacing.Relaxed => 2,
        _ => 1.6,
    };

    public void Dispose()
    {
        _readingSettings.SettingsChanged -= ApplySettings;
        _documentService.DocumentChanged -= OnDocumentChanged;
        _autoSaveTimer?.Stop();
    }
}
