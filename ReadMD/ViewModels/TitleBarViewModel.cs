using Avalonia.Controls;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentIcons.Common;
using ReadMD.Models;
using ReadMD.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ReadMD.ViewModels;

public partial class TitleBarViewModel : ViewModelBase, IDisposable
{
    private readonly IThemeService _themeService;
    private readonly IReadingSettingsService _readingSettings;
    private readonly IWindowService _windowService;
    private readonly IFileDialogService _fileDialogService;
    private readonly IMarkdownDocumentService _markdownDocumentService;
    private readonly ILocalizationService _localization;

    [ObservableProperty] private string _appTitle = "ReadMD";
    [ObservableProperty] private bool _isDarkTheme;
    [ObservableProperty] private int _selectedLanguageIndex;
    [ObservableProperty] private Icon _maximizeIcon = Icon.Maximize;

    // Настройки чтения
    [ObservableProperty] private LineWidth _lineWidth;
    [ObservableProperty] private bool _useSerifs;
    [ObservableProperty] private int _fontSize;
    [ObservableProperty] private LineSpacing _lineSpacing;

    public UiStrings Texts => _localization.Strings;

    public TitleBarViewModel(
        IThemeService themeService,
        IReadingSettingsService readingSettings,
        IWindowService windowService,
        IFileDialogService fileDialogService,
        IMarkdownDocumentService markdownDocumentService,
        ILocalizationService localization)
    {
        _themeService = themeService;
        _readingSettings = readingSettings;
        _windowService = windowService;
        _fileDialogService = fileDialogService;
        _markdownDocumentService = markdownDocumentService;
        _localization = localization;

        _isDarkTheme = themeService.CurrentTheme == ThemeVariant.Dark;
        _selectedLanguageIndex = LanguageToIndex(_localization.CurrentLanguage);
        AppTitle = _localization.Strings.FileNamePlaceholder;

        SyncFromService();
        _readingSettings.SettingsChanged += SyncFromService;
        _localization.LanguageChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, EventArgs e) =>
        OnPropertyChanged(nameof(Texts));

    private void SyncFromService()
    {
        LineWidth = _readingSettings.LineWidth;
        UseSerifs = _readingSettings.UseSerifs;
        FontSize = _readingSettings.FontSize;
        LineSpacing = _readingSettings.LineSpacing;

        OnPropertyChanged(nameof(LineWidth));
        OnPropertyChanged(nameof(UseSerifs));
        OnPropertyChanged(nameof(FontSize));
        OnPropertyChanged(nameof(LineSpacing));
    }

    partial void OnLineWidthChanged(LineWidth value) => _readingSettings.LineWidth = value;
    partial void OnUseSerifsChanged(bool value) => _readingSettings.UseSerifs = value;
    partial void OnFontSizeChanged(int value) => _readingSettings.FontSize = value;
    partial void OnLineSpacingChanged(LineSpacing value) => _readingSettings.LineSpacing = value;

    partial void OnSelectedLanguageIndexChanged(int value) =>
        _localization.SetLanguage(IndexToLanguage(value));

    [RelayCommand]
    private void Minimize() => _windowService.Minimize();

    [RelayCommand]
    private void Maximize()
    {
        _windowService.Maximize();
        MaximizeIcon = _windowService.GetWindowState() == WindowState.Maximized
            ? Icon.SquareMultiple
            : Icon.Maximize;
    }

    [RelayCommand]
    private void Close() => _windowService.Close();

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var path = await _fileDialogService.ShowOpenMarkdownFileDialogAsync();
        if (path is null) return;

        var markdown = await File.ReadAllTextAsync(path);
        _markdownDocumentService.Markdown = markdown;
        AppTitle = Path.GetFileName(path);
    }

    [RelayCommand]
    private void DecreaseFontSize() { if (FontSize > 10) FontSize--; }

    [RelayCommand]
    private void IncreaseFontSize() { if (FontSize < 32) FontSize++; }

    partial void OnIsDarkThemeChanged(bool value) =>
        _themeService.SetTheme(value ? ThemeVariant.Dark : ThemeVariant.Light);

    public void Dispose()
    {
        _readingSettings.SettingsChanged -= SyncFromService;
        _localization.LanguageChanged -= OnLanguageChanged;
    }

    private static int LanguageToIndex(AppLanguage language) => language switch
    {
        AppLanguage.Russian => 1,
        AppLanguage.English => 2,
        _ => 0
    };

    private static AppLanguage IndexToLanguage(int index) => index switch
    {
        1 => AppLanguage.Russian,
        2 => AppLanguage.English,
        _ => AppLanguage.System
    };
}
