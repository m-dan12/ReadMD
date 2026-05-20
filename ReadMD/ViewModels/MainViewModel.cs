using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ExCSS;
using ReadMD.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ReadMD.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IReadingSettingsService _readingSettings;
    private readonly IMarkdownDocumentService _markdownDocumentService;
    private DispatcherTimer? _autoSaveTimer;
    
    // Задержка автосохранения в миллисекундах (2 секунды)
    private const int AutoSaveDelayMs = 2000;

    // ── Ширина строки ────────────────────────────────────────────────
    [ObservableProperty] private double lineWidthWidth = 700;
    [ObservableProperty] private double lineWidthMaxWidth = 850;

    // ── Документ ─────────────────────────────────────────────────────
    [ObservableProperty] private string markdown = "# Заголовок";
    [ObservableProperty] private string markdownSource = "# Заголовок";
    [ObservableProperty] private bool isEditMode;

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
        markdownSource = markdown;

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

    partial void OnMarkdownSourceChanged(string value)
    {
        // В режиме редактирования обновляем превью в реальном времени
        if (IsEditMode)
        {
            Markdown = value;
            
            // Запускаем таймер автосохранения при каждом изменении
            RestartAutoSaveTimer();
        }
    }

    partial void OnIsEditModeChanged(bool value)
    {
        if (value)
        {
            // Входим в режим редактирования: копируем текущий markdown в редактор
            MarkdownSource = Markdown;
        }
        else
        {
            // Выходим из режима редактирования
            Markdown = MarkdownSource;
            
            // Останавливаем таймер автосохранения
            StopAutoSaveTimer();
            
            // Сохраняем в файл сразу при выходе из режима
            if (_markdownDocumentService.FilePath is not null)
            {
                _markdownDocumentService.Markdown = MarkdownSource;
                SaveFileAsync();
            }
        }
    }

    private async void SaveFileAsync()
    {
        try
        {
            var filePath = _markdownDocumentService.FilePath;
            if (filePath is not null)
            {
                // Небольшая задержка для завершения всех UI операций
                await Task.Delay(100);
                await File.WriteAllTextAsync(filePath, MarkdownSource);
            }
        }
        catch
        {
            // Ошибка при сохранении - игнорируем
        }
    }
    
    /// <summary>
    /// Перезапускает таймер автосохранения. Срабатывает при каждом изменении текста.
    /// </summary>
    private void RestartAutoSaveTimer()
    {
        // Останавливаем старый таймер
        _autoSaveTimer?.Stop();
        
        // Создаем новый таймер с задержкой AutoSaveDelayMs
        _autoSaveTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(AutoSaveDelayMs)
        };
        
        _autoSaveTimer.Tick += (_, _) =>
        {
            StopAutoSaveTimer();
            
            // Сохраняем текущие изменения
            if (_markdownDocumentService.FilePath is not null)
            {
                _markdownDocumentService.Markdown = MarkdownSource;
                SaveFileAsync();
            }
        };
        
        _autoSaveTimer.Start();
    }
    
    /// <summary>
    /// Останавливает и очищает таймер автосохранения.
    /// </summary>
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
}