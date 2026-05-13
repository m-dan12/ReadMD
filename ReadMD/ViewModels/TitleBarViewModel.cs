using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentIcons.Common;
using ReadMD.Services;

namespace ReadMD.ViewModels;

public partial class TitleBarViewModel : ViewModelBase
{
    private readonly IThemeService _themeService;
    private readonly IReadingSettingsService _readingSettings;
    private readonly IWindowService _windowService;

    // Существующие свойства
    [ObservableProperty] private string _appTitle = "ReadMD";
    [ObservableProperty] private bool _isDarkTheme;
    [ObservableProperty] private bool _isEnglish = true;
    [ObservableProperty] private bool _isRussian;
    [ObservableProperty] private Icon _maximizeIcon = Icon.Maximize;

    // Новое свойство для ширины строки
    [ObservableProperty]
    private LineWidth _lineWidth;

    public TitleBarViewModel(
        IThemeService themeService,
        IReadingSettingsService readingSettings,
        IWindowService windowService)
    {
        _themeService = themeService;
        _readingSettings = readingSettings;
        _windowService = windowService;

        _isDarkTheme = themeService.CurrentTheme == ThemeVariant.Dark;
        _lineWidth = readingSettings.LineWidth;

        // Подписываемся на изменения в сервисе (на случай изменений из других мест)
        _readingSettings.SettingsChanged += OnReadingSettingsChanged;
    }

    private void OnReadingSettingsChanged()
    {
        LineWidth = _readingSettings.LineWidth;
    }

    // Синхронизация изменений из ViewModel обратно в сервис
    partial void OnLineWidthChanged(LineWidth value)
    {
        _readingSettings.LineWidth = value;
    }


    [RelayCommand]
    private void Minimize() => _windowService.Minimize();

    [RelayCommand]
    private void Maximize()
    {
        _windowService.Maximize();
        MaximizeIcon = _windowService.getWindowState() == WindowState.Maximized ? Icon.SquareMultiple : Icon.Maximize;
    }

    [RelayCommand]
    private void Close() => _windowService.Close();

    partial void OnIsDarkThemeChanged(bool value)
    {
        _themeService.SetTheme(value ? ThemeVariant.Dark : ThemeVariant.Light);
    }

    [RelayCommand]
    private void SetLanguage(string lang)
    {
        IsEnglish = lang == "en";
        IsRussian = lang == "ru";
        // Здесь можно добавить логику смены языка приложения
    }

    // Освобождение подписки при уничтожении ViewModel (хорошая практика)
    public void Dispose()
    {
        if (_readingSettings != null)
        {
            _readingSettings.SettingsChanged -= OnReadingSettingsChanged;
        }
    }
}